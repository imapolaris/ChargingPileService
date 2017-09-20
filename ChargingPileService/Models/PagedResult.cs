using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChargingPileService.Models
{
    [Serializable]
    public class PagedResult<T>: MulResult<T>
    {
        public PagedResult():base()
        {

        }

        public int PageSize { get; set; }

        public int StartIndex { get; set; }

        public int TotalCount { get; set; }
    }
}
