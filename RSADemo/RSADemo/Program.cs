using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 使用RSA来判断文档是否被修改过。
/// </summary>
namespace RSADemo
{
    class Program
    {
        private CngKey roseKey;
        private byte[] rosePubKeyBlob;

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        public void Run()
        {
            byte[] document;
            byte[] hash;
            byte[] signature;

            RoseTasks(out document, out hash, out signature);
            //document[3] = 0x34; //此处修改，用以验证是否能检测出文档是否被修改。
            BobTasks(document, hash, signature);
            Console.ReadKey();
        }

        public void RoseTasks(out byte[] data, out byte[] hash, out byte[] signature)
        {
            roseKey = CngKey.Create(CngAlgorithm.Rsa);
            rosePubKeyBlob = roseKey.Export(CngKeyBlobFormat.GenericPublicBlob);

            data = Encoding.UTF8.GetBytes("Best greetings from Rose.");
            //获取哈希值
            hash = HashDocument(data);
            signature = AddSignatureToHash(hash, roseKey);
        }

        private byte[] HashDocument(byte[] data)
        {
            using (var hashAlg = SHA384.Create())
            {
                return hashAlg.ComputeHash(data);
            }
        }

        /// <summary>
        /// 为Hash签名
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] AddSignatureToHash(byte[] hash, CngKey key)
        {
            using (var signingAlg = new RSACng(key))
            {
                byte[] signed = signingAlg.SignHash(hash, HashAlgorithmName.SHA384, RSASignaturePadding.Pss);
                return signed;
            }
        }


        public void BobTasks(byte[] data, byte[] hash, byte[] signature)
        {
            CngKey _roseKey = CngKey.Import(rosePubKeyBlob, CngKeyBlobFormat.GenericPublicBlob);

            if (!IsSignatureValid(hash, signature, _roseKey))
            {
                Console.WriteLine("signature not valid");
                return;
            }

            if (!IsDocumentUnchanged(hash, data))
            {
                Console.WriteLine("document was changed");
                return;
            }
            Console.WriteLine("signature valid, document unchanged");
            Console.WriteLine($"document from Rose:{Encoding.UTF8.GetString(data)}");
        }

        private bool IsSignatureValid(byte[] hash, byte[] signature, CngKey key)
        {
            using (var signingAlg = new RSACng(key))
            {
                return signingAlg.VerifyHash(hash,signature,HashAlgorithmName.SHA384, RSASignaturePadding.Pss);
            }
        }

        private bool IsDocumentUnchanged(byte[] hash, byte[] data)
        {
            byte[] newHash = HashDocument(data);
            return newHash.SequenceEqual(hash);
        }

    }
}
