using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class LoginResultPacket : PacketBase
    {
        private LoginResult _result;
        /// <summary>
        /// 登录结果
        /// 1、登录成功；2、设备不存在；3、已经登录；4、密钥失效；5、其他错误
        /// </summary>
        public LoginResult ResultEnum
        {
            get { return _result; }
            set { _result = value; }
        }

        public byte Result
        {
            get
            {
                return (byte)_result;
            }
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

        public LoginResultPacket(PacketType command) : base(PacketType.LoginResult)
        {
        }

        public override byte[] Encode()
        {
            byte[] body = new byte[BodyLen];

            int start = 0;

            byte[] temp = base.Encode();
            Array.Copy(temp, 0, body, start, SerialNumberLen);
            start += SerialNumberLen;
            temp = BitConverter.GetBytes(this.Result);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 1;
            temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);

            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            int start = 0;
            base.Decode(buffer);
            start += SerialNumberLen;
            this._result = (LoginResult)buffer[start];
            start += 1;
            this._timestamp = BitConverter.ToInt32(buffer, start);

            return this;
        }
    }
}
