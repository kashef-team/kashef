 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace Promotion.Service.API
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            //Log Exception
            LogException(actionExecutedContext.Exception);

            //Handle Exception
            
        }

        private void LogException(Exception ex)
        {
            //Logger.LogException(ex);
        }
    }
}