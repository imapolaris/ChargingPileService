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
    [Serializable]
    public class StationDetail : EntityBase
    {
        private string stationId;
        private double price;
        private string introduce;

        [StringLength(50)]
        [Required]
        public string StationId
        {
            get { return stationId; }
            set { stationId = value; }
        }

        /// <summary>
        /// 电价
        /// </summary>
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        /// <summary>
        /// 电站介绍
        /// </summary>
        public string Introduce
        {
            get { return introduce; }
            set { introduce = value; }
        }
    }
}
