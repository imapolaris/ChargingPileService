using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChargingPileService.Models
{
    [Serializable]
    public class SingleResult<T> : ResultBase
    {
        public SingleResult(T t)
        {
            result = t;
        }

        private T result;
        [JsonProperty]
        public T Result
        {
            get { return result; }
            set { result = value; }
        }

        [NonSerialized]
        private int statusCode = 0;
        [JsonIgnore]
        public int StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }
    }
}
