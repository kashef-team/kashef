using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Security
{
    /// <summary>
    /// ICryptography is responsible about encryption and decryption..
    /// </summary>
    public interface ICryptography
    {
        /// <summary>
        /// encrypt plain string using tripleDES algorithm
        /// </summary>
        /// <param name="plainString">the plain string</param>
        /// <param name="cryptographyKey">the key used for encryption</param>
        /// <returns></returns>
        string Encrypt(string plainString, string cryptographyKey);


        /// <summary>
        /// Decrypt encrpted string using tripleDES algorithm
        /// </summary>
        /// <param name="encryptedString">the encrypted string</param>
        /// <param name="cryptographyKey">the key used for decryption</param>
        /// <returns></returns>
        string Decrypt(string encryptedString, string cryptographyKey);
    }
}
