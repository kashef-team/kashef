
using Kashef.API.Filters;
using Kashef.API.Models;
using Kashef.API.Results;
using Kashef.Services;
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

        #endregion

        #region Constructors

        public DetectorController(IRekognitionService RekognitionService)
        {
            if (null == RekognitionService)
            {
                throw new ArgumentNullException();
            }
            _rekognitionService = RekognitionService;
        }

        #endregion

        #region Action Methods

        [HttpGet]
        [Route("api/v1/Kashef/Detection/Terrorists/find")]
        [ModelValidationFilter]
        public void Get(string imageURl)
        {
            //Get Image
            WebClient webClient = new WebClient();
            byte[] imageBinary = webClient.DownloadData(imageURl);

            //Create Image Model ...
            Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image(); 
            image.Bytes = new MemoryStream(imageBinary); 

            var result = _rekognitionService.Recognize(Constants.WhiteListCollectionId, image); 

            //Create Cache Control Header...
            CacheControlHeaderValue cacheControlHeader = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = new TimeSpan(0, 2, 0)
            };

            //return new OkResultWithCaching<Project>(project, this)
            //{
            //    CacheControlHeader = cacheControlHeader
            //};
        }
        
        [HttpGet]
        [Route("api/v1/Kashef/whitelist/add")] 
        public IHttpActionResult Post(string imageURl)
        {
            //Get Image
            WebClient webClient = new WebClient();
            byte[] imageBinary = webClient.DownloadData(imageURl);

            //Create Image Model ...
            Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image();
            image.Bytes = new MemoryStream(imageBinary);

            bool result = _rekognitionService.AddFacesToCollection(Constants.WhiteListCollectionId, image);

            if (result)
            {
                return new HttpActionResult(System.Net.HttpStatusCode.Created);
            }
            else
            {
                return BadRequest("Failed to add the face to whilelist collection");
            }
        }

        [HttpGet]
        [Route("api/v1/Kashef/blacklist/add")] 
        public IHttpActionResult Add(string imageURl)
        {
            //Get Image
            WebClient webClient = new WebClient();
            byte[] imageBinary = webClient.DownloadData(imageURl);

            //Create Image Model ...
            Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image();
            image.Bytes = new MemoryStream(imageBinary);

            bool result = _rekognitionService.AddFacesToCollection(Constants.BlackListCollectionId, image);

            if (result)
            {
                return new HttpActionResult(System.Net.HttpStatusCode.Created);
            }
            else
            {
                return BadRequest("Failed to add the face to backlist collection");
            }
        }

        #endregion
    }
}