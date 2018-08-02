using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kashef.Services
{
    public class CollectionManagmentService : ICollectionService
    {
        #region Constructors

        public CollectionManagmentService()
        {

        }

        #endregion

        #region Functions

        public void AddCollection(string collectionId)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();

            CreateCollectionRequest createCollectionRequest = new CreateCollectionRequest()
            {
                CollectionId = collectionId
            };

            CreateCollectionResponse createCollectionResponse = rekognitionClient.CreateCollection(createCollectionRequest);
        }

        public List<string> GetAllFacesInCollection(string collectionId)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();

            ListFacesResponse listFacesResponse = null;

            List<string> faces = new List<string>();

            string paginationToken = null;
            do
            {
                if (listFacesResponse != null)
                {
                    paginationToken = listFacesResponse.NextToken;
                }

                ListFacesRequest listFacesRequest = new ListFacesRequest()
                {
                    CollectionId = collectionId,
                    MaxResults = 1,
                    NextToken = paginationToken
                };

                listFacesResponse = rekognitionClient.ListFaces(listFacesRequest);
                foreach (Amazon.Rekognition.Model.Face face in listFacesResponse.Faces)
                {
                    faces.Add(face.FaceId);
                }
            }
            while (listFacesResponse != null && !String.IsNullOrEmpty(listFacesResponse.NextToken));

            return faces;
        }

        public void RemoveFacesFromCollection(string collectionId, List<string> removedFaces)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();

            DeleteFacesRequest deleteFacesRequest = new DeleteFacesRequest()
            {
                CollectionId = collectionId,
                FaceIds = removedFaces
            };

            DeleteFacesResponse deleteFacesResponse = rekognitionClient.DeleteFaces(deleteFacesRequest);
        }

        #endregion
    }
}