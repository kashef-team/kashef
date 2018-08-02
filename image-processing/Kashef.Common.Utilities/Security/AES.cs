using Kashef.Common.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Security
{
    /// <summary>
    /// Implementation for Icryptograpy using AES Algorithm.
    /// </summary>
    public class AES : ICryptography
    { 
        #region Properties

        private int _iterationNumber = 1000;

        #endregion

        #region Public Methods
        /// <summary>
        /// Encrypt plain string using tripleDES algorithm
        /// </summary>
        /// <param name="plainString">the plain string</param>
        /// <param name="cryptographyKey">the key used for encryption</param>
        /// <returns></returns>
        public string Encrypt(string plainString, string cryptographyKey)
        {
            //Precondition check
            if (String.IsNullOrEmpty(plainString))
            {
                throw new ArgumentNullException("plainstring", Resources.Exception_InvalidTraceMessage);
            }

            if (String.IsNullOrEmpty(cryptographyKey))
            {
                throw new ArgumentNullException("cryptographyKey", Resources.Exception_InvalidTraceMessage);
            }

            //Convert Key into Array of bytes... 
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(cryptographyKey);

            // Hash key bytes with SHA256
            keyBytes = SHA256.Create().ComputeHash(keyBytes);

            // Getting the salt size
            int saltSize = GetSaltSize(keyBytes);

            // Generating salt bytes
            byte[] saltBytes = GetRandomBytes(saltSize);

            //Convert plain text into Bytes... 
            byte[] originalBytes = Encoding.UTF8.GetBytes(plainString);

            //Appeand Salt Bytes into original Bytes
            byte[] encryptedBytes = GetBytesToEncrypt(saltBytes, originalBytes);

            //Encrypt
            return Convert.ToBase64String(Encrypt(encryptedBytes, keyBytes));
        }

        /// <summary>
        /// Decrypt encrpted string using tripleDES algorithm
        /// </summary>
        /// <param name="encryptedString">the encrypted string</param>
        /// <param name="cryptographyKey">the key used for decryption</param>
        /// <returns></returns>
        public string Decrypt(string encryptedString, string cryptographyKey)
        {
            if (String.IsNullOrEmpty(encryptedString))
            {
                throw new ArgumentNullException("encryptedString", Resources.Exception_InvalidTraceMessage);
            }

            if (String.IsNullOrEmpty(cryptographyKey))
            {
                throw new ArgumentNullException("cryptographyKey", Resources.Exception_InvalidTraceMessage);
            }

            //Convert Key into Bytes...
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(cryptographyKey);

            // Hash the password with SHA256
            keyBytes = SHA256.Create().ComputeHash(keyBytes);

            //Convert Encrypted String into bytes.
            byte[] bytesToBeDecrypted = Convert.FromBase64String(encryptedString);

            //Decrypt 
            byte[] decryptedBytes = Decrypt(bytesToBeDecrypted, keyBytes);

            // Get the size of salt.
            int saltSize = GetSaltSize(keyBytes);

            // Removing salt bytes, retrieving original bytes
            byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
            for (int i = saltSize; i < decryptedBytes.Length; i++)
            {
                originalBytes[i - saltSize] = decryptedBytes[i];
            }

            return Encoding.UTF8.GetString(originalBytes);
        }


        #endregion
        
        #region Private Methods
  
        /// <summary>
        /// Generate Random Bytes 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] GetRandomBytes(int length)
        {
            //Define Array
            byte[] randomBytes = new byte[length];

            //Generate Array of Random Bytes.
            RNGCryptoServiceProvider.Create().GetBytes(randomBytes);
            return randomBytes;
        }

        private byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] keyBytes)
        { 
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged aesManaged = new RijndaelManaged())
                {
                    //Set Key Size
                    aesManaged.KeySize = 256;

                    //Set Block Size
                    aesManaged.BlockSize = 128;

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(keyBytes, keyBytes, _iterationNumber);

                    //Set Key...
                    aesManaged.Key = key.GetBytes(aesManaged.KeySize / 8);

                    //Set Vector
                    aesManaged.IV = key.GetBytes(aesManaged.BlockSize / 8);

                    //Set Mode
                    aesManaged.Mode = CipherMode.CBC;

                    //Encrypt
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesManaged.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cryptoStream.Close();
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        private byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] keyBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged aesManaged = new RijndaelManaged())
                {
                    //Set Key Size
                    aesManaged.KeySize = 256;

                    //Set Block Size
                    aesManaged.BlockSize = 128;

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(keyBytes, keyBytes, _iterationNumber);

                    //Set Key...
                    aesManaged.Key = key.GetBytes(aesManaged.KeySize / 8);

                    //Set Vector
                    aesManaged.IV = key.GetBytes(aesManaged.BlockSize / 8);

                    //Set Mode
                    aesManaged.Mode = CipherMode.CBC;

                    //Decrypt
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesManaged.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cryptoStream.Close();
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Get Bytes To Encrypt by appeanding salt to original 
        /// </summary>
        /// <param name="saltBytes"></param>
        /// <param name="originalBytes"></param>
        /// <returns></returns>
        private byte[] GetBytesToEncrypt(byte[] saltBytes, byte[] originalBytes)
        {
            byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
            for (int i = 0; i < saltBytes.Length; i++)
            {
                bytesToBeEncrypted[i] = saltBytes[i];
            }
            for (int i = 0; i < originalBytes.Length; i++)
            {
                bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
            }
            return bytesToBeEncrypted;
        }

        /// <summary>
        /// Get Salt Size based on key bytes.
        /// </summary>
        /// <param name="keyBytes"></param>
        /// <returns></returns>
        private int GetSaltSize(byte[] keyBytes)
        {
            return 32;
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(keyBytes, keyBytes, _iterationNumber);
            byte[] ba = key.GetBytes(2);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < ba.Length; i++)
            {
                builder.Append(Convert.ToInt32(ba[i]).ToString());
            }
            int saltSize = 0;
            string s = builder.ToString();
            foreach (char c in s)
            {
                int intc = Convert.ToInt32(c.ToString());
                saltSize = saltSize + intc;
            }

            return saltSize;
        }

        #endregion
    }
}
