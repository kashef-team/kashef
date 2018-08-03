
using Amazon.Rekognition.Model;
using Kashef.API.Filters;
using Kashef.API.Models;
using Kashef.API.Results;
using Kashef.Common.Utilities.Diagnostics;
using Kashef.Services;
using Kashef.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kashef.API.Controllers
{ 
    public class DetectorController : ApiController
    {
        #region Properties

        private IRekognitionService _rekognitionService = null;

        private ICollectionService _collectionService = null;

        #endregion

        #region Constructors

        public DetectorController(IRekognitionService RekognitionService, ICollectionService collectionService)
        {
            if (null == RekognitionService || null == collectionService)
            {
                throw new ArgumentNullException();
            }
            _rekognitionService = RekognitionService;
            _collectionService = collectionService;
        }

        #endregion

        #region Action Methods

        [HttpGet]
        [Route("api/v1/Kashef/Detection/Terrorists/find")]
        [ModelValidationFilter]
        public IHttpActionResult Get(string imageURl)
        {
            //Get Image
            WebClient webClient = new WebClient();
            byte[] imageBinary = webClient.DownloadData(imageURl);

            //Create Image Model ...
            Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image(); 
            image.Bytes = new MemoryStream(imageBinary);

            List<FaceRecord> blackListResult = _rekognitionService.Recognize(Constants.BlackListCollectionId, image);

            List<FaceRecord> missingResuList = _rekognitionService.Recognize(Constants.WhiteListCollectionId, image);

            RecognizResult result = new RecognizResult
            {
                BlackListFaces = blackListResult,
                MissingListFaces = missingResuList
            };

            //Create Cache Control Header...
            CacheControlHeaderValue cacheControlHeader = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = new TimeSpan(0,0, 1)
            };

            return new OkResultWithCaching<RecognizResult>(result, this)
            {
                CacheControlHeader = cacheControlHeader
            };
        }
         
        [HttpGet]
        [Route("api/v1/Kashef/whitelist/get")]
        public IHttpActionResult GetFacesOfWhiteList()
        {
            try
            {
                List<string> result = _collectionService.GetAllFacesInCollection(Constants.WhiteListCollectionId);

                //Create Cache Control Header...
                CacheControlHeaderValue cacheControlHeader = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = new TimeSpan(0, 0, 1)
                };

                return new OkResultWithCaching<List<string>>(result, this)
                {
                    CacheControlHeader = cacheControlHeader
                };
            }
            catch(Exception ex)
            {
                return new OkResultWithCaching<Exception>(ex, this)
                { 
                };
            }
        }
         
        [HttpGet]
        [Route("api/v1/Kashef/blacklist/get")]
        public IHttpActionResult GetFacesOfBlackList()
        {
            try
            {
                List<string> result = _collectionService.GetAllFacesInCollection(Constants.BlackListCollectionId);

                //Create Cache Control Header...
                CacheControlHeaderValue cacheControlHeader = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = new TimeSpan(0, 0, 1)
                };

                return new OkResultWithCaching<List<string>>(result, this)
                {
                    CacheControlHeader = cacheControlHeader
                };
            }
            catch (Exception ex)
            {
                return new OkResultWithCaching<Exception>(ex, this)
                {
                };
            }
        }

        [HttpPost]
        [Route("api/v1/Kashef/whitelist/add")]
        public IHttpActionResult AddToWhiteList(string imageURl)
        {
            try
            {
                //Get Image
                WebClient webClient = new WebClient();
                byte[] imageBinary = webClient.DownloadData(imageURl);

                //Create Image Model ...
                Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image();
                image.Bytes = new MemoryStream(imageBinary);

                FaceRecord result = _rekognitionService.AddImageToCollection(Constants.WhiteListCollectionId, image);

                if (null != result)
                {
                    return new OkResultWithCaching<FaceRecord>(result, this)
                    {
                    };
                }
                else
                {
                    return BadRequest("Failed to add the face to whilelist collection");
                }
            }
            catch (ArgumentNullException exception)
            {
                Logger.LogException(exception);
                return BadRequest("Many faces in the frame");
            }
            catch(Exception exception)
            {
                Logger.LogException(exception);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("api/v1/Kashef/blacklist/add")]
        public IHttpActionResult AddToBlackList(string imageURl)
        {
            try
            {
                //Get Image
                WebClient webClient = new WebClient();
                byte[] imageBinary = webClient.DownloadData(imageURl);

                //Create Image Model ...
                Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image();
                image.Bytes = new MemoryStream(imageBinary);

                FaceRecord result = _rekognitionService.AddImageToCollection(Constants.BlackListCollectionId, image);

                if ( null != result)
                {
                    return new OkResultWithCaching<FaceRecord>(result, this)
                    {
                    };
                }
                else
                {
                    return BadRequest("Failed to add the face to backlist collection");
                }
            }
            catch (ArgumentNullException exception)
            {
                Logger.LogException(exception);
                return BadRequest("Many faces in the frame");
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                return BadRequest();
            }
        } 

        [HttpPost]
        [Route("api/v1/Kashef/collection/add")]
        public IHttpActionResult AddCollection(string collectionId)
        {
            if (string.IsNullOrEmpty(collectionId))
            {
                return BadRequest("Invalid collectionId");
            }

            bool result = _collectionService.AddCollection(collectionId);

            if (result)
            {
                return new HttpActionResult(System.Net.HttpStatusCode.Created);
            }
            else
            {
                return BadRequest("Failed to add the face to whilelist collection");
            }
        }

        [HttpPost]
        [Route("api/v1/Kashef/collection/remove")]
        public IHttpActionResult RemoveCollection(string collectionId)
        {
            if (string.IsNullOrEmpty(collectionId))
            {
                return BadRequest("Invalid collectionId");
            }

            //Get All Faces ...
            List<string> facesId = _collectionService.GetAllFacesInCollection(collectionId);

            //Remove All Faces from the collection...
            _collectionService.RemoveFacesFromCollection(collectionId, facesId);


            return new HttpActionResult(System.Net.HttpStatusCode.OK);
        }

        #endregion
    }
}