using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class LoginPacket
    {
        private int _timeStamp;
        /// <summary>
        /// 当前时间
        /// </summary>
        public int TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        private string _username;
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }


        private string _pwd;
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }

    }
}
