using Kashef.Common.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Security
{
    /// <summary>
    /// TripleDES responsible about encrypt/decrypt.
    /// </summary>
    public sealed class TripleDES : ICryptography
    {
        /// <summary>
        /// Encrypt plain string using tripleDES algorithm
        /// </summary>
        /// <param name="plainString">the plain string</param>
        /// <param name="cryptographyKey">the key used for encryption</param>
        /// <returns></returns>
        public string Encrypt(string plainString, string cryptographyKey)
        {
            if (String.IsNullOrEmpty(plainString))
                throw new ArgumentNullException("plainstring", Resources.Exception_InvalidTraceMessage);
            if (String.IsNullOrEmpty(cryptographyKey))
                throw new ArgumentNullException("cryptographyKey", Resources.Exception_InvalidTraceMessage);

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(plainString);

            //generate key array
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(cryptographyKey));
            hashmd5.Clear();
            //Set Provider parameters
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            //Encrypt the string
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// Decrypt encrpted string using tripleDES algorithm
        /// </summary>
        /// <param name="encryptedString">the encrypted string</param>
        /// <param name="cryptographyKey">the key used for decryption</param>
        /// <returns></returns>
        public  string Decrypt(string encryptedString, string cryptographyKey)
        {
            if (String.IsNullOrEmpty(encryptedString))
                throw new ArgumentNullException("encryptedString", Resources.Exception_InvalidTraceMessage);
            if (String.IsNullOrEmpty(cryptographyKey))
                throw new ArgumentNullException("cryptographyKey", Resources.Exception_InvalidTraceMessage);

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(encryptedString);

            //Generate Key Array
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(cryptographyKey));
            hashmd5.Clear();

            //Set Provider parameters
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            //Decrypt the string
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
