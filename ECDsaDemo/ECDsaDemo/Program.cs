using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ECDSA加密的使用
/// </summary>
namespace ECDsaDemo
{
    class Program
    {
        private CngKey _roseKeySignature;
        private byte[] _rosePubKeyBlob;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        private void InitRoseKeys()
        {
            _roseKeySignature = CngKey.Create(CngAlgorithm.ECDsaP521);
            _rosePubKeyBlob = _roseKeySignature.Export(CngKeyBlobFormat.GenericPublicBlob);
        }

        /// <summary>
        /// 给数据签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] CreateSignature(byte[] data, CngKey key)
        {
            byte[] signature;
            using (var signingAlg = new ECDsaCng(key))
            {
                signature = signingAlg.SignData(data);
                signingAlg.Clear();
            }
            return signature;
        }


        public bool VerifySignature(byte[] data, byte[] signature, byte[] pubKey)
        {
            bool retValue = false;

            using (CngKey key = CngKey.Import(pubKey, CngKeyBlobFormat.GenericPublicBlob))
            using (var signingAlg = new ECDsaCng(key))
            {
                retValue = signingAlg.VerifyData(data, signature);
                signingAlg.Clear();
            }
            return retValue;
        }

        public void Run()
        {
            InitRoseKeys();
            byte[] roseData = Encoding.UTF8.GetBytes("I am Rose");
            //为数据加上签名
            byte[] roseSignatureData = CreateSignature(roseData, _roseKeySignature);

            Console.WriteLine($"Rose created signature: {Convert.ToBase64String(roseSignatureData)}");

            //使用生成的公钥去解密数据
            if (VerifySignature(roseData, roseSignatureData, _rosePubKeyBlob))
            {
                Console.WriteLine("Rose signature verified successfully.");
                Console.WriteLine($"解密：{Encoding.UTF8.GetString(roseData)}");
            }

        }

    }
}
