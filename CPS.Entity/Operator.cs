using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    /// <summary>
    /// 运营商
    /// </summary>
    public class Operator : EntityBase
    {
        /// <summary>
        /// 运营商编号
        /// </summary>
        public int SerialNo { get; set; }
        public string SerialNoString
        {
            get
            {
                return this.SerialNo.ToString().PadLeft(3, '0');
            }
        }
    }
}
