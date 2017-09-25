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
    public class ResultBase
    {
        public ResultBase(bool r, string m)
        {
            result = r;
            message = m;
        }

        private bool result;
        [JsonProperty]
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        private string message;
        [JsonProperty]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
