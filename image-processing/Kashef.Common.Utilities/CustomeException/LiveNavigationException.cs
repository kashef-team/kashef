using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks; 

namespace Kashef.Common.Utilities.CustomeException
{
    public class LiveNavigationException : Exception, ISerializable
    {

        public string ErrorCode { get; set; }

        public LiveNavigationException(string errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }
        public LiveNavigationException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
        public LiveNavigationException(string errorCode, string message, System.Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        protected LiveNavigationException(string errorCode, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorCode = errorCode;
        }
    }


    public class LiveNavigationEntityValidationException : Exception, ISerializable
    {

        public string ErrorCode { get; set; }

        public LiveNavigationEntityValidationException(string errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }
        public LiveNavigationEntityValidationException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
        public LiveNavigationEntityValidationException(string errorCode, string message, System.Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        protected LiveNavigationEntityValidationException(string errorCode, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorCode = errorCode;
        }
    }

}