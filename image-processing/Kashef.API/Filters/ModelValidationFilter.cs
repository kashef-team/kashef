using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Kashef.API.Filters
{ 
    public class ModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                //var errors = actionContext.ModelState
                //                          .Values
                //                          .SelectMany(m => m.Errors
                //                                            .Select(e => e.ErrorMessage));

                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    (HttpStatusCode)422, actionContext.ModelState);
            }
        }
    }
}