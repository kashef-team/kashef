using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Security
{
    public static class CertificateLoader
    {
        public static X509Certificate2 GetX509Certificate2BySubjectName(string subjectName)
        {
            //Define Certificate Instance...
            X509Certificate2 certificate = null;

            //Create Certificate Store Object...
            X509Store store = new X509Store(StoreLocation.LocalMachine);

            //Open Certificate store as read only Mode...
            store.Open(OpenFlags.ReadOnly);

            //Get all certificates in store...
            X509Certificate2Collection certCollection = store.Certificates;

            // Loop through each certificate and find the certificate with the appropriate name.
            foreach (X509Certificate2 cert in certCollection)
            {
                if (cert.Subject == subjectName)
                {
                    certificate = cert;
                    break;
                }
            }

            store.Close();
            return certificate;
        }

        public static X509Certificate2 GetX509CertificateByThumbprint(string thumbprint)
        {
            //Define Certificate Instance...
            X509Certificate2 certificate = null;

            //Create Certificate Store Object...
            X509Store store = new X509Store(StoreLocation.LocalMachine);

            //Open Certificate store as read only Mode...
            store.Open(OpenFlags.ReadOnly);

            //Get all certificates in store...
            X509Certificate2Collection certCollection = store.Certificates;

            // Loop through each certificate and find the certificate with the appropriate name.
            foreach (X509Certificate2 cert in certCollection)
            {
                if (cert.Thumbprint.ToLower() == thumbprint.ToLower())
                {
                    certificate = cert;
                    break;
                }
            }

            store.Close();
            return certificate;
        }
    }
}
