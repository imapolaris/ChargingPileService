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
            BodyLen = PacketBase.SerialNumberLen + 4;
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

        public override PacketBase Decode(byte[] buffer)
        {
            base.Decode(buffer);
            int start = SerialNumberLen;
            this._timestamp = BitConverter.ToInt32(buffer, start);
            return this;
        }

        public override byte[] Encode()
        {
            byte[] body = base.Encode();
            int start = SerialNumberLen;
            byte[] temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, 4);

            return body;
        }
    }
}
