using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    /// <summary>
    /// 系统字典
    /// </summary>
    public class SysDic : EntityBase
    {
        private string key;
        /// <summary>
        /// 键
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        private string val;
        /// <summary>
        /// 值
        /// </summary>
        public string Val
        {
            get { return val; }
            set { val = value; }
        }

        private string unit;
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        private int order;
        /// <summary>
        /// 顺序
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; }
        }
    }
}
