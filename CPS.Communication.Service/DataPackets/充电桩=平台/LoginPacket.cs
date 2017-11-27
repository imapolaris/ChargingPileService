using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class LoginPacket : PacketBase
    {
        public LoginPacket() : base(PacketType.Login)
        {
            BodyLen = PacketBase.HeaderLen + PacketBase.SerialNumberLen + 24;
        }

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

        public override byte[] Encode()
        {
            byte[] body = new byte[BodyLen];

            int start = 0;

            byte[] temp = base.Encode();
            Array.Copy(temp, 0, body, start, SerialNumberLen);
            start += SerialNumberLen;
            temp = BitConverter.GetBytes(this._timeStamp);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = EncodeHelper.GetBytes(this._username);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            temp = EncodeHelper.GetBytes(this._pwd);
            Array.Copy(temp, 0, body, start, temp.Length);

            return body;
        }

        public override PacketBase Decode(byte[] buffer)
        {
            int start = 0;
            base.Decode(buffer);
            start += SerialNumberLen;
            this._timeStamp = BitConverter.ToInt32(buffer, start);
            start += 4;
            byte[] un = new byte[8];
            Array.Copy(buffer, start, un, 0, un.Length);
            this._username = EncodeHelper.GetString(un);
            start += 8;
            byte[] pwd = new byte[12];
            Array.Copy(buffer, start, pwd, 0, pwd.Length);
            this._pwd = EncodeHelper.GetString(pwd);

            return this;
        }
    }
}
