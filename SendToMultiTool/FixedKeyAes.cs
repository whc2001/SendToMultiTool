using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SendToMultiTool
{
    internal class FixedKeyAes
    {
        public static Dictionary<byte, byte[]> KeyTable { get; private set; } = new Dictionary<byte, byte[]>();

        public static string KeyDeriveSalt { get; private set; } = "20090828JX3";

        public static void InitKeys()
        {
            byte[] salt = Encoding.ASCII.GetBytes(FixedKeyAes.KeyDeriveSalt);
            for (int keyIndex = 0; keyIndex <= 0xFF; ++keyIndex)
            {
                Rfc2898DeriveBytes keyGen = new Rfc2898DeriveBytes(new byte[1] { (byte)keyIndex }, salt, 1024);
                FixedKeyAes.KeyTable[(byte)keyIndex] = keyGen.GetBytes(16);
                keyGen.Dispose();
            }
        }

        public static byte[] Encrypt(byte[] payload)
        {
            byte keyIndex = Utils.XorHash(payload);
            byte[] key = FixedKeyAes.KeyTable[keyIndex];
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.KeySize = 128;
                aes.Padding = PaddingMode.ISO10126;
                aes.Key = key;
                aes.GenerateIV();
                using (ICryptoTransform aesEnc = aes.CreateEncryptor())
                {
                    List<byte> ret = new List<byte>();
                    ret.AddRange(Encoding.ASCII.GetBytes("ENCRYPT"));
                    ret.Add(keyIndex);
                    ret.AddRange(aes.IV);
                    ret.AddRange(aesEnc.TransformFinalBlock(payload, 0, payload.Length));
                    return ret.ToArray();
                }
            }
        }

        public static byte[] Decrypt(byte[] encrypted)
        {
            string signature = Encoding.ASCII.GetString(encrypted.SubArray(0, 7));
            if (signature != "ENCRYPT")
                throw new ArgumentException($"数据头无效: {signature}");
            byte keyIndex = encrypted[7];
            byte[] key = FixedKeyAes.KeyTable[keyIndex];
            byte[] iv = encrypted.SubArray(8, 16);
            byte[] encryptedPayload = encrypted.SubArray(24, encrypted.Length - 24);
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.KeySize = 128;
                aes.Padding = PaddingMode.ISO10126;
                aes.Key = key;
                aes.IV = iv;
                using (ICryptoTransform aesDec = aes.CreateDecryptor())
                {
                    return aesDec.TransformFinalBlock(encryptedPayload, 0, encryptedPayload.Length);
                }
            }
        }
    }
}
