using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Entities
{
    public class MyMessage : EntityBase
    {
        private string userId;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private string msgId;
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MsgId
        {
            get { return msgId; }
            set { msgId = value; }
        }

        private bool hasChecked;
        /// <summary>
        /// 是否已查看
        /// </summary>
        public bool HasChecked
        {
            get { return hasChecked; }
            set { hasChecked = value; }
        }

    }
}
