using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class LoginResultPacket
    {
        private short _result;
        /// <summary>
        /// 登录结果
        /// 1、登录成功；2、设备不存在；3、已经登录；4、密钥失效；5、其他错误
        /// </summary>
        public short Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private int _timestamp;
        /// <summary>
        /// 当前时间
        /// </summary>
        public int TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

    }
}
