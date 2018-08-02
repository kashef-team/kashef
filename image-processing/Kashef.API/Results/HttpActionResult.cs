using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Kashef.API.Results
{
    public class HttpActionResult : IHttpActionResult
    {
        private readonly string _message;
        private readonly HttpStatusCode _statusCode;

        public HttpActionResult(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        public HttpActionResult(HttpStatusCode statusCode, string message)
        {
            _statusCode = statusCode;
            _message = message;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(_statusCode)
            { 
             //   Content = string.IsNullOrEmpty(_message) ? null : new StringContent(new JsonSerializer().Serialize<ErrorMessage>(new ErrorMessage(_message)))
            };


            return Task.FromResult(response);
        }

      
    }
}