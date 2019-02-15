using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSADemo
{
    class Program
    {
        private CngKey roseKey;
        private byte[] rosePubKeyBlob;

        static void Main(string[] args)
        {
        }

        public void Run()
        {
            byte[] document;
            byte[] hash;
            byte[] signature;

        }

        public void RoseTasks(out byte[] data, out byte[] hash, out byte[] signature)
        {
            roseKey = CngKey.Create(CngAlgorithm.Rsa);
            rosePubKeyBlob = roseKey.Export(CngKeyBlobFormat.GenericPublicBlob);

            data = Encoding.UTF8.GetBytes("Best greetings from Rose.");
            //获取哈希值
            hash = HashDocument(data);

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

            }
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
