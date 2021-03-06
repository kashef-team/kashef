﻿using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Kashef.Common.Utilities.Diagnostics;
using Kashef.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kashef.Services
{
    public class RekognitionService : IRekognitionService
    {
        #region Properties

        private ICollectionService _collectionService = null;

        #endregion

        #region Constructors

        public RekognitionService(ICollectionService collectionService)
        {
            if(null == collectionService)
            {
                throw new ArgumentNullException();
            }

            _collectionService = collectionService;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Add All detected faces to a specific collection 
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="imageId"></param>
        /// <param name="image"></param>
        public FaceRecord AddImageToCollection(string collectionId, Amazon.Rekognition.Model.Image image)
        {
            AmazonRekognitionClient rekognitionClient = AmazonClient.GetInstance();

            //Validate that image contains only one face.
            DetectFacesResponse detectFacesResponse = rekognitionClient.DetectFaces(new Amazon.Rekognition.Model.DetectFacesRequest
            {
                Attributes = new List<string> { "ALL" },
                Image = image

            });

            if (null != detectFacesResponse.FaceDetails && detectFacesResponse.FaceDetails.Count > 1)
            {
                throw new ArgumentNullException("Many faces in the image");
            }

            IndexFacesRequest indexFacesRequest = new IndexFacesRequest()
            {

                Image = image,
                CollectionId = collectionId,
                DetectionAttributes = new List<String>() { "ALL" }
            };

            IndexFacesResponse indexFacesResponse = rekognitionClient.IndexFaces(indexFacesRequest);

            return indexFacesResponse.FaceRecords.FirstOrDefault();
        }
         
        public List<FaceRecord> Recognize(string collectionId, Amazon.Rekognition.Model.Image image)
        {

            //1- Detect faces in the input image and adds them to the specified collection. 
            AmazonRekognitionClient rekognitionClient = AmazonClient.GetInstance();

            IndexFacesRequest indexFacesRequest = new IndexFacesRequest()
            {
                Image = image,
                CollectionId = collectionId,
                DetectionAttributes = new List<String>() { "DEFAULT" }
            };

            IndexFacesResponse indexFacesResponse = rekognitionClient.IndexFaces(indexFacesRequest);

            //2- Search all detected faces in the collection
            SearchFacesResponse searchFacesResponse = null;

            List<FaceRecord> matchedFaces = new List<FaceRecord>();

            if (null != indexFacesResponse && null != indexFacesResponse.FaceRecords && 0 != indexFacesResponse.FaceRecords.Count)
            {
                foreach (FaceRecord face in indexFacesResponse.FaceRecords)
                {
                    searchFacesResponse = rekognitionClient.SearchFaces(new SearchFacesRequest
                    {
                        CollectionId = collectionId,
                        FaceId = face.Face.FaceId,
                        FaceMatchThreshold = 70F,
                        MaxFaces = 2
                    });

                    if (searchFacesResponse.FaceMatches != null && searchFacesResponse.FaceMatches.Count != 0)
                    {
                        matchedFaces.Add(face);
                    }
                }

                //Remove newly added faces to the collection

              _collectionService.RemoveFacesFromCollection(collectionId, indexFacesResponse.FaceRecords.Select(x => x.Face.FaceId).ToList());

            }

            return matchedFaces;
        }


        #endregion

    }
}