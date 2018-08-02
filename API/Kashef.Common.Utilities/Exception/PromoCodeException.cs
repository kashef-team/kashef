using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PromoCode.Common.Utilities.Exception
{
    public class PromoCodeException : System.Exception, ISerializable
    {

        public string ErrorCode { get; set; }

        public PromoCodeException(string errorCode) : base()
        {
            ErrorCode = errorCode;
        }
        public PromoCodeException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
        public PromoCodeException(string errorCode, string message, System.Exception inner) : base(message, inner)
        {
            ErrorCode = errorCode;
        } 
        
        protected PromoCodeException(string errorCode, SerializationInfo info, StreamingContext context) : base (info, context)
        {
            ErrorCode = errorCode;
        }
    }
}