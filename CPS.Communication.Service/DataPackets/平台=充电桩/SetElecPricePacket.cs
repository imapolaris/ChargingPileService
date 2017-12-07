using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class SetElecPricePacket : OperPacketBase
    {
        public SetElecPricePacket() : base(PacketTypeEnum.SetElecPrice)
        {
            BodyLen = OperPacketBodyLen + 4 + 4 + 4 + 4;
        }

        public SetElecPricePacket(PacketTypeEnum pte) : base(pte) { }

        private int _sharpRate;
        /// <summary>
        /// 尖费率
        /// </summary>
        public int SharpRate
        {
            get { return _sharpRate; }
            set { _sharpRate = value; }
        }

        /// <summary>
        /// 单位：厘
        /// </summary>
        public double SharpRateVal
        {
            get
            {
                return this._sharpRate * RateCoefficient;
            }
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

        public double PeakRateVal
        {
            get
            {
                return this._peakRate * RateCoefficient;
            }
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

        public double FlatRateVal
        {
            get
            {
                return this._flatRate * RateCoefficient;
            }
        }

        private int _valleyRate;
        /// <summary>
        /// 谷费率
        /// </summary>
        public int ValleyRate
        {
            get { return _valleyRate; }
            set { _valleyRate = value; }
        }

        public double ValleyRateVal
        {
            get
            {
                return this._valleyRate * RateCoefficient;
            }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            byte[] temp = BitConverter.GetBytes(this._sharpRate);
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
            this._sharpRate = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._peakRate = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._flatRate = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._valleyRate = BitConverter.ToInt32(buffer, start);
            return this;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
