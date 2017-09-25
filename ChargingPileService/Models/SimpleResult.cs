using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService.Models
{
    [Serializable]
    public class SimpleResult : ResultBase
    {
        public SimpleResult(bool r=true, string m="")
            : base(r, m)
        {
        }
    }
}