using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kashef.Services
{
    public interface IRekognitionService
    {
        FaceRecord AddImageToCollection(string collectionId, Amazon.Rekognition.Model.Image image);

        List<FaceRecord> Recognize(string collectionId, Amazon.Rekognition.Model.Image image);
    }
}