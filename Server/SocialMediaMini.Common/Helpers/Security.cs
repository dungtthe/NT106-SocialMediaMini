using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.Helpers
{
    
    public static class Security
    {
        public static string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); 
                }
                return sb.ToString(); 
            }
        }

        //Nao xây sau
        public static string Encrypt(string plainText)
        {
           return plainText;
        }

        //Nao xây sau
        public static string Decrypt(string cipherText)
        {
            return cipherText;
        }
    }
}
