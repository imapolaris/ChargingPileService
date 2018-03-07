using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Exceptions
{
    public class UniversalException : Exception
    {
        public UniversalException()
        {
        }

        public UniversalException(string message) : base(message)
        {
        }

        public UniversalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UniversalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
