using Amazon.Rekognition;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Services
{
    public static class AmazonClient
    {
        public static AmazonRekognitionClient GetInstance()
        {
            BasicAWSCredentials credentials = new BasicAWSCredentials("ID", "Value");

            return new AmazonRekognitionClient(credentials, Amazon.RegionEndpoint.USWest2);

        }
    }
}
