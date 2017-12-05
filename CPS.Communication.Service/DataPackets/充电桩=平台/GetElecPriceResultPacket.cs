using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class GetElecPriceResultPacket : OperPacketBase
    {
        public GetElecPriceResultPacket() : base(PacketTypeEnum.GetElecPriceResult)
        {
            BodyLen = OperPacketBodyLen + 4 + 4 + 4 + 4 + 4;
        }

        public GetElecPriceResultPacket(PacketTypeEnum pet) : base(pet)
        {

        }

        private int _sharpRate;
        /// <summary>
        /// 尖费率
        /// </summary>
        public int SharpRate
        {
            get { return _sharpRate; }
            set { _sharpRate = value; }
        }

        private int _peakRate;
        /// <summary>
        /// 峰费率
        /// </summary>
        public int PeakRate
        {
            get { return _peakRate; }
            set { _peakRate = value; }
        }

        private int _flatRate;
        /// <summary>
        /// 平费率
        /// </summary>
        public int FlatRate
        {
            get { return _flatRate; }
            set { _flatRate = value; }
        }

        private int _valleyRate;
        /// <summary>
        /// 谷费率
        /// </summary>
        public int Valleyrate
        {
            get { return _valleyRate; }
            set { _valleyRate = value; }
        }

        private int _timestamp;
        /// <summary>
        /// 充电桩当前时间
        /// </summary>
        public int TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = new byte[BodyLen];
            int start = OperPacketBodyLen;
            byte[] temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._sharpRate);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._peakRate);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._flatRate);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._valleyRate);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._timestamp = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._sharpRate = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._peakRate = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._flatRate = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._valleyRate = BitConverter.ToInt32(buffer, start);
            return this;
        }
    }
}
