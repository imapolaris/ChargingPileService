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
        public SingleResult(bool r, string m, T d)
            : base(r, m)
        {
            data = d;
        }

        private T data;
        public T Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
