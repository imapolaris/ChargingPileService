using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    /// <summary>
    /// 充电桩
    /// </summary>
    public class ChargingPile : EntityBase
    {
        /// <summary>
        /// 电站Id
        /// </summary>
        public string StationId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Category { get; set; }
    }
}
