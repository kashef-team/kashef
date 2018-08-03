using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kashef.API.Models
{
    public class RecognizResult
    {
        public List<FaceRecord> BlackListFaces
        {
            get; set;
        }

        public List<FaceRecord> MissingListFaces
        {
            get; set;
        }
    }
}