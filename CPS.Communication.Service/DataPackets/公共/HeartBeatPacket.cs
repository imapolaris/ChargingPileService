using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class HeartBeatPacket : PacketBase
    {
        public HeartBeatPacket(PacketTypeEnum pt)
             : base(pt)
        {
            BodyLen = 4;
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

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._timestamp = BitConverter.ToInt32(buffer, start);
            return this;
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = 0;
            byte[] temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);

            return body;
        }
    }
}
