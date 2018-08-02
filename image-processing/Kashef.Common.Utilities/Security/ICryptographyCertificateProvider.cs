using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Security
{
    public interface ICryptographyCertificateProvider
    {

        string Encrypt(string plainText, X509Certificate2 certificate);

        string Decrypt(string cipherText, X509Certificate2 certificate);

        string Sign(string text, X509Certificate2 certificate);

        bool Verify(string originalString, string hashedString, X509Certificate2 certificate);
    }
}