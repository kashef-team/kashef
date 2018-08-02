//using LiveNavigation.Common.Resource;
//using LiveNavigation.DataAccess.Core.Context;
//using LiveNavigation.DataAccess.Models.Base;
//using LiveNavigation.DataAccess.Models.Custom;
//using LiveNavigation.DataAccess.Models.DTO;
//using LiveNavigation.DataAccess.Repositories.Implementation;
//using Kashef.API.Filters;
//using Kashef.API.Results;
//using LiveNavigation.Services; 
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace Kashef.API.Controllers
//{
//    [Authorize]
//    public class CustomerController : ApiController
//    {
//        #region Properties

//        private ISalesService _SalesService = null;

//        #endregion

//        #region Constructors
//        public CustomerController(ISalesService SalesServiceInstance)
//        {
//            if (null == SalesServiceInstance)
//            {
//                throw new ArgumentNullException();
//            }
//            _SalesService = SalesServiceInstance;
//        }

//        #endregion

//        #region Action Methods

//        [HttpGet]
//        [Route("api/v1/LiveNavigation/Customer/get")]
//        [ModelValidationFilter]
//        public IHttpActionResult Get(string emailAddressOrPhoneNumber)
//        {
//            CustomerDTO customer = _SalesService.GetCustomerByEmailAddressOrPhonenNumber(emailAddressOrPhoneNumber);

//            //Create Cache Control Header...
//            CacheControlHeaderValue cacheControlHeader = new CacheControlHeaderValue()
//            {
//                Public = true,
//                MaxAge = new TimeSpan(0, 2, 0)
//            };

//            return new OkResultWithCaching<CustomerDTO>(customer, this)
//            {
//                CacheControlHeader = cacheControlHeader
//            };
//        }
         
//        #endregion
//    }
//}