using System;
using System.Security.Cryptography;
using WebApiCore.Common;

namespace WebApiCore.Utility
{
    public static class PasswordHelper
    {
        private static byte[] GenerateSalt()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[Constant.Salt]);

            return salt;
        }

        private static byte[] GenerateHash(string text)
        {
            var salt = GenerateSalt();
            var pbkdf2 = new Rfc2898DeriveBytes(text, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(Constant.Hash);

            byte[] hashBytes = new byte[Constant.PasswordHash];
            Array.Copy(salt, 0, hashBytes, 0, Constant.Salt);
            Array.Copy(hash, 0, hashBytes, Constant.Salt, Constant.Hash);

            return hashBytes;
        }        

        public static string DecodePassword(string text)
        {
            return Convert.ToBase64String(GenerateHash(text));
        }

        public static bool ComparePassword(string inputPass, string storePass)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(storePass);

            /* Get the salt */
            byte[] salt = new byte[Constant.Salt];
            Array.Copy(hashBytes, 0, salt, 0, Constant.Salt);

            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(inputPass, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            /* Compare the results */
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i]) return false;
            }

            return true;
        }
    }
}
