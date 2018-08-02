using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Kashef.API.Models
{
    public static class HTTPRequestHelper
    {
        public static string GetRequestETagHeader(HttpRequestMessage request)
        {
            string requestEtag = null;
            var requestETagHeader = request.Headers.FirstOrDefault(x => x.Key.Equals("ETag"));
            if (null != requestETagHeader.Value)
            {
                requestEtag = requestETagHeader.Value.FirstOrDefault().ToString();
            }
            return requestEtag;
        }
    }
}