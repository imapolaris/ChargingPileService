using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    public class Wallet : EntityBase
    {
        /// <summary>
        /// 余额
        /// </summary>
        public double Remaining { get; set; }
    }
}
