using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Exceptions
{
    public class UnConnectException : Exception
    {
        public UnConnectException()
        {
        }

        public UnConnectException(string message) : base(message)
        {
        }

        public UnConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
