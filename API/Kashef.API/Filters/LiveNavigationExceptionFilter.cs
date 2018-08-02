//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;

//namespace Kashef.API.Filters
//{
//    public class ExceptionFilter : ExceptionFilterAttribute
//    {
//        public override void OnException(HttpActionExecutedContext actionExecutedContext)
//        {
//            //Log Exception
//            LogException(actionExecutedContext.Exception);

//            //Handle Exception
//            if (actionExecutedContext.Exception is LiveNavigationEntityValidationException)
//            {
//                LiveNavigationEntityValidationException promoCodeEntityValidationException = actionExecutedContext.Exception as LiveNavigationEntityValidationException;

//                actionExecutedContext.Response = new HttpResponseMessage()
//                {
//                    Content = new StringContent(new JsonSerializer().Serialize<ErrorMessage>(new ErrorMessage(promoCodeEntityValidationException.ErrorCode))),
//                    StatusCode = (HttpStatusCode)422
//                };
//            }
//            else
//            {
//                actionExecutedContext.Response = new HttpResponseMessage()
//                {
//                    Content = new StringContent(new JsonSerializer().Serialize<ErrorMessage>(new ErrorMessage(actionExecutedContext.Exception.Message))),
//                    StatusCode = HttpStatusCode.InternalServerError
//                };
//            }
//        }

//        private void LogException(Exception ex)
//        {
//            Logger.LogException(ex);
//        }
//    }
//}