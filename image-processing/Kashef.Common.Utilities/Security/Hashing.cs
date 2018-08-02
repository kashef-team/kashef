using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Security
{
    public static class Hashing
    { 
        //Get MD5 hashing string ...
        public static string GetMD5HashString(string plainText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in GetHash(plainText))
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string CreateHashedPassword(string plainPassword)
        {
            SHA1CryptoServiceProvider sha1Provider = new SHA1CryptoServiceProvider();

            //Get Password as bytes array
            byte[] passwordBytes = Encoding.ASCII.GetBytes(plainPassword);

            //Hash Password
            byte[] hashedPasswordBytes = sha1Provider.ComputeHash(passwordBytes);

            //Get Hashed Password
            return ASCIIEncoding.ASCII.GetString(hashedPasswordBytes);
        }
    }
}
