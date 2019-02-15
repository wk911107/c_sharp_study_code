using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// EC Diffie-Hellman算法来安全传递数据的Demo
/// </summary>
namespace SecureTransferDemo
{
    class Program
    {
        private CngKey jackKey;
        private CngKey roseKey;

        private byte[] jackPubKeyBlob;
        private byte[] rosePubKeyBlob;

        public void CreateKeys()
        {
            roseKey = CngKey.Create(CngAlgorithm.ECDiffieHellmanP521);
            jackKey = CngKey.Create(CngAlgorithm.ECDiffieHellmanP521);

            rosePubKeyBlob = roseKey.Export(CngKeyBlobFormat.EccPublicBlob);
            jackPubKeyBlob = jackKey.Export(CngKeyBlobFormat.EccPublicBlob);

        }

        public async Task<byte[]> RoseSendsDataAsync(string msg)
        {
            Console.WriteLine($"Rose's message is {msg}");
            byte[] rawData = Encoding.UTF8.GetBytes(msg);


            byte[] encryptedData = null;
            using (var roseAlgorithm = new ECDiffieHellmanCng(roseKey))
            using (CngKey jackPubKey = CngKey.Import(jackPubKeyBlob,CngKeyBlobFormat.EccPublicBlob))
            {
                //使用Jack的公钥和Rose的密钥对生成一个对称密钥
                byte[] symmKey = roseAlgorithm.DeriveKeyMaterial(jackPubKey);

                Console.WriteLine($"The symmetric key is {Convert.ToBase64String(symmKey)}");

                using (var aes = new AesCryptoServiceProvider())
                {
                    aes.Key = symmKey;
                    aes.GenerateIV();
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (var ms = new MemoryStream())
                    {
                        //创建CryptoStream来加密数据并发送
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            await ms.WriteAsync(aes.IV, 0, aes.IV.Length);
                            cs.Write(rawData, 0, rawData.Length);
                        }
                        encryptedData = ms.ToArray();
                    }
                    aes.Clear();
                }

            }

            Console.WriteLine($"the ecrypted data is {Convert.ToBase64String(encryptedData)}");

            Console.WriteLine();
            return encryptedData;

        }

        public async Task JackRecvDataAsync(byte[] encryptedData)
        {
            Console.WriteLine("Bob recevies encrypted data");
            byte[] rawData = null;

            var aes = new AesCryptoServiceProvider();

            int nBytes = aes.BlockSize >> 3;

            byte[] iv = new byte[nBytes];

            for (int i = 0; i < iv.Length; i++)
            {
                iv[i] = encryptedData[i];
            }

            using (var bobAlgorithm = new ECDiffieHellmanCng(jackKey))
            using (CngKey rosePubKey = CngKey.Import(rosePubKeyBlob, CngKeyBlobFormat.EccPublicBlob))
            {
                byte[] symmKey = bobAlgorithm.DeriveKeyMaterial(rosePubKey);
                Console.WriteLine($"The Symmetric key is {Convert.ToBase64String(symmKey)}");

                aes.Key = symmKey;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (MemoryStream ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(encryptedData, nBytes, encryptedData.Length - nBytes);
                    }
                    rawData = ms.ToArray();

                    Console.WriteLine($"Decrypts message is :{Encoding.UTF8.GetString(rawData)}");
                }
                aes.Clear();
            }
        }


        public async Task RunAsync()
        {
            try
            {
                CreateKeys();
                byte[] encryptData = await RoseSendsDataAsync("This is the secret message for Jack, i love you!");
                await JackRecvDataAsync(encryptData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Main(string[] args)
        {
            var p = new Program();
            p.RunAsync().Wait();
            Console.ReadLine();
        }
    }
}
