using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    /// <summary>
    /// 电站详情
    /// </summary>
    public class StationDetail : EntityBase
    {
        private string stationId;

        [StringLength(50)]
        public string StationId
        {
            get { return stationId; }
            set { stationId = value; }
        }
    }
}
