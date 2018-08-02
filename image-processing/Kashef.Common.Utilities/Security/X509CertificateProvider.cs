using Kashef.Common.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Security
{
   public class X509CertificateProvider : ICryptographyCertificateProvider
    {
        public string Encrypt(string plainText, X509Certificate2 certificate)
        {
            //Precondition check... 
            if (certificate == null || string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException(Resources.InValidEncryptionParams);
            } 

            RSACryptoServiceProvider rsaEncryptor = (RSACryptoServiceProvider)certificate.PublicKey.Key;
         
            byte[] cipherData = rsaEncryptor.Encrypt(Encoding.UTF8.GetBytes(plainText), true);

            return Convert.ToBase64String(cipherData);
        }

        public string Decrypt(string cipherText, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            //Precondition check... 
            if (certificate == null || string.IsNullOrEmpty(cipherText))
            {
                throw new Exception(Resources.InValidDecryptionParams);
            }

            if (!certificate.HasPrivateKey)
            {
                throw new Exception(Resources.CertificateHasNoPrivateKey);
            }
             
            RSACryptoServiceProvider rsaEncryptor = (RSACryptoServiceProvider)certificate.PrivateKey;
            
            byte[] plainData = rsaEncryptor.Decrypt(Convert.FromBase64String(cipherText), true);

            return Encoding.UTF8.GetString(plainData);
        }

        public string Sign(string text, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            //Precondition check... 
            if (certificate == null || string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(Resources.InValidSigningParams);
            }

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;

            // Hash the data
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(text);
            byte[] hash = sha1.ComputeHash(data);


            // Sign the hash
            byte[] signedData = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));

            return Convert.ToBase64String(signedData);
        }

        public bool Verify(string originalString, string hashedString, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {

            //Precondition check... 
            if (certificate == null || string.IsNullOrEmpty(originalString) || string.IsNullOrEmpty(hashedString))
            {
                throw new ArgumentNullException(Resources.InValidSigningParams);
            }



            // Get its associated CSP and public key
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            // Hash the data 
            SHA1Managed sha1 = new SHA1Managed();
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(originalString);
            byte[] hash = sha1.ComputeHash(data);

            byte[] signature = Convert.FromBase64String(hashedString);

            // Verify the signature with the hash
            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);

        }
    }
}
