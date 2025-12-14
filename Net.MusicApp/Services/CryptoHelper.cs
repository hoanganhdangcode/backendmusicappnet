using System.Security.Cryptography;
using System.Text;

namespace Net.MusicApp.Services
{

    public static class CryptoHelper
    {


        #region Sinh keyaes256 nếu chưa có

        private static byte[] LoadOrGenerateKeyAES()
        {
            var filePath = "./Keys/aes_key.txt";
            byte[] key;
            if (File.Exists(filePath))
            {
                string base64 = File.ReadAllText(filePath);
                key = Convert.FromBase64String(base64);
            }
            else
            {
                key = RandomNumberGenerator.GetBytes(32); // AES-256
                File.WriteAllText(filePath, Convert.ToBase64String(key));
            }
            return key;
        }
        #endregion

        public static string HashSHA256(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        //public static string HashMD5(string input)
        //{
        //    using var md5 = System.Security.Cryptography.MD5.Create();
        //    var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        //    var hash = md5.ComputeHash(bytes);
        //    return Convert.ToBase64String(hash);
        //}
        public static string EncryptAES256(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText)) return "";

            using var aes = Aes.Create();
            aes.Key = LoadOrGenerateKeyAES();
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // prepend IV + cipher, lưu Base64
            var result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }


        public static string DecryptAES256(string cipherTextBase64)
        {
            if (string.IsNullOrWhiteSpace(cipherTextBase64)) return "";
            var fullCipher = Convert.FromBase64String(cipherTextBase64);

            var iv = new byte[16]; // AES block size
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var aes = Aes.Create();
            aes.Key = LoadOrGenerateKeyAES();
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(decrypted);
        }



    }
}
