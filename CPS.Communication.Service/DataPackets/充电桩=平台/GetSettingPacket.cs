using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetSettingPacket : PacketBase
    {
        public GetSettingPacket() : base(PacketTypeEnum.GetSetting)
        {
            BodyLen = 4;
        }

        private int _timeStamp;

        public int TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = 0;
            byte[] temp = BitConverter.GetBytes(this._timeStamp);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._timeStamp = BitConverter.ToInt32(buffer, start);
            return this;
        }
    }
}
