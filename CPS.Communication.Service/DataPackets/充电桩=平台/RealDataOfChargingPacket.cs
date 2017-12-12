using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class RealDataOfChargingPacket : PacketBase
    {
        public RealDataOfChargingPacket() : base(PacketTypeEnum.RealDataOfCharging)
        {
            BodyLen = 8 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 2 + 1 + 1 + 1 + 2 + 4 + 4 + 4 + 4 + 4 + 17;
        }

        private long _transactionSN;
        /// <summary>
        /// 交易流水号
        /// </summary>
        public long TransactionSN
        {
            get { return _transactionSN; }
            set { _transactionSN = value; }
        }

        private byte _qport;
        /// <summary>
        /// 充电桩接口
        /// </summary>
        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }

        private int _totalElec;
        /// <summary>
        /// 当前充电总电量
        /// </summary>
        public int TotalElec
        {
            get { return _totalElec; }
            set { _totalElec = value; }
        }

        private int _sharpElec;
        /// <summary>
        /// 当前充电尖电量
        /// </summary>
        public int SharpElec
        {
            get { return _sharpElec; }
            set { _sharpElec = value; }
        }

        private int _peakElec;
        /// <summary>
        /// 当前充电峰电量
        /// </summary>
        public int PeakElec
        {
            get { return _peakElec; }
            set { _peakElec = value; }
        }

        private int _flatElec;
        /// <summary>
        /// 当前充电平电量
        /// </summary>
        public int FlatElec
        {
            get { return _flatElec; }
            set { _flatElec = value; }
        }

        private int _valleyElec;
        /// <summary>
        /// 当前充电谷电量
        /// </summary>
        public int ValleyElec
        {
            get { return _valleyElec; }
            set { _valleyElec = value; }
        }

        private int _elecMoney  ;
        /// <summary>
        /// 当前充电金额
        /// </summary>
        public int ElecMoney    
        {
            get { return _elecMoney; }
            set { _elecMoney = value; }
        }

        private int _serviceMoney;
        /// <summary>
        /// 当前服务费金额
        /// </summary>
        public int ServiceMoney
        {
            get { return _serviceMoney; }
            set { _serviceMoney = value; }
        }

        private short _costTime;
        /// <summary>
        /// 当前充电时间
        /// </summary>
        public short CostTime
        {
            get { return _costTime; }
            set { _costTime = value; }
        }

        private byte _cpState;
        /// <summary>
        /// 充电桩状态
        /// </summary>
        public byte CpState
        {
            get { return _cpState; }
            set { _cpState = value; }
        }

        private byte _stopReason;
        /// <summary>
        /// 停止充电原因
        /// </summary>
        public byte StopReason
        {
            get { return _stopReason; }
            set { _stopReason = value; }
        }

        private byte _soc;
        /// <summary>
        /// 当前SOC
        /// </summary>
        public byte SOC
        {
            get { return _soc; }
            set { _soc = value; }
        }

        private short _surplusTime;
        /// <summary>
        /// 剩余充电时间
        /// </summary>
        public short SurplusTime
        {
            get { return _surplusTime; }
            set { _surplusTime = value; }
        }

        private int _outputV;
        /// <summary>
        /// 输出电压
        /// </summary>
        public int OutputV
        {
            get { return _outputV; }
            set { _outputV = value; }
        }

        private int _outputA;
        /// <summary>
        /// 输出电流
        /// </summary>
        public int OutputA
        {
            get { return _outputA; }
            set { _outputA = value; }
        }

        private int _minTemp;
        /// <summary>
        /// 电池组最低温度
        /// </summary>
        public int MinTemp
        {
            get { return _minTemp; }
            set { _minTemp = value; }
        }

        private int _maxTemp;
        /// <summary>
        /// 电池组最高温度
        /// </summary>
        public int MaxTemp
        {
            get { return _maxTemp; }
            set { _maxTemp = value; }
        }

        private int _timestamp;
        /// <summary>
        /// 充电桩当前时间戳
        /// </summary>
        public int TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        private string _vin;
        /// <summary>
        /// 车辆VIN号
        /// </summary>
        public string Vin
        {
            get { return _vin; }
            set { _vin = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = new byte[BodyLen];
            int start = 0;
            byte[] temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            body[start] = this._qport;
            start += 1;
            temp = BitConverter.GetBytes(this._totalElec);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._sharpElec);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._peakElec);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._flatElec);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._valleyElec);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._elecMoney);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._serviceMoney);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._costTime);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 2;
            body[start] = this._cpState;
            start += 1;
            body[start] = this._stopReason;
            start += 1;
            body[start] = this._soc;
            start += 1;
            temp = BitConverter.GetBytes(this._surplusTime);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 2;
            temp = BitConverter.GetBytes(this._outputV);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._outputA);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._minTemp);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._maxTemp);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = EncodeHelper.GetBytes(this._vin);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._qport = buffer[start];
            start += 1;
            this._totalElec = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._sharpElec = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._peakElec = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._flatElec = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._valleyElec = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._elecMoney = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._serviceMoney = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._costTime = BitConverter.ToInt16(buffer, start);
            start += 2;
            this._cpState = buffer[start];
            start += 1;
            this._stopReason = buffer[start];
            start += 1;
            this._soc = buffer[start];
            start += 1;
            this._surplusTime = BitConverter.ToInt16(buffer, start);
            start += 2;
            this._outputV = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._outputA = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._minTemp = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._maxTemp = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._timestamp = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._vin = EncodeHelper.GetString(buffer, start, 17);
            return this;
        }
    }
}
