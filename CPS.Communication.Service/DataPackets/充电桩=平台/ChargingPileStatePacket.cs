using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPS.Infrastructure.Models;

namespace CPS.Communication.Service.DataPackets
{
    public class ChargingPileStatePacket : PacketBase, IUniversal
    {
        public ChargingPileStatePacket() : base(PacketTypeEnum.ChargingPileState)
        {
            BodyLen = 1 + 1 + 1 + 1 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 1 + 1 + 4 + 4 + 4 + 4 + 4 + 1 + 1 + 4;
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

        private byte _subscribeState;
        /// <summary>
        /// 预约状态
        /// </summary>
        public byte SubscribeState
        {
            get { return _subscribeState; }
            set { _subscribeState = value; }
        }

        private byte _carportState;
        /// <summary>
        /// 车位状态
        /// </summary>
        public byte CarPortState
        {
            get { return _carportState; }
            set { _carportState = value; }
        }

        private byte _connectState;
        /// <summary>
        /// 接口连接状态
        /// </summary>
        public byte ConnectState
        {
            get { return _connectState; }
            set { _connectState = value; }
        }

        private byte _workingState;
        /// <summary>
        /// 接口工作状态
        /// </summary>
        public byte WorkingState
        {
            get { return _workingState; }
            set { _workingState = value; }
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

        private int _aphaseV;
        /// <summary>
        /// A相电压
        /// </summary>
        public int APhaseV
        {
            get { return _aphaseV; }
            set { _aphaseV = value; }
        }

        private int _bphaseV;
        /// <summary>
        /// B相电压
        /// </summary>
        public int BPhaseV
        {
            get { return _bphaseV; }
            set { _bphaseV = value; }
        }

        private int _cphaseV;
        /// <summary>
        /// C相电压
        /// </summary>
        public int CPhaseV
        {
            get { return _cphaseV; }
            set { _cphaseV = value; }
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

        private int _aphaseA;
        /// <summary>
        /// A相电流
        /// </summary>
        public int APhaseA
        {
            get { return _aphaseA; }
            set { _aphaseA = value; }
        }

        private int _bphaseA;
        /// <summary>
        /// B相电流
        /// </summary>
        public int BPhaseA
        {
            get { return _bphaseA; }
            set { _bphaseA = value; }
        }

        private int _cphaseA;
        /// <summary>
        /// C相电流
        /// </summary>
        public int CPhaseA
        {
            get { return _cphaseA; }
            set { _cphaseA = value; }
        }

        private byte _outputRelayState;
        /// <summary>
        /// 输出继电器状态
        /// </summary>
        public byte OutputRelayState
        {
            get { return _outputRelayState; }
            set { _outputRelayState = value; }
        }

        private byte _wpgWorkingState;
        /// <summary>
        /// 系统风机工作状态
        /// </summary>
        public byte WpgWorkingState
        {
            get { return _wpgWorkingState; }
            set { _wpgWorkingState = value; }
        }

        private int _rtTemp;
        /// <summary>
        /// 充电桩实时温度
        /// </summary>
        public int RtTemp
        {
            get { return _rtTemp; }
            set { _rtTemp = value; }
        }

        public double RtTempVal
        {
            get
            {
                return this._rtTemp * 0.001;
            }
        }

        private int _p;
        /// <summary>
        /// 有功功率
        /// </summary>
        public int P
        {
            get { return _p; }
            set { _p = value; }
        }

        private int _q;
        /// <summary>
        /// 无功功率
        /// </summary>
        public int Q
        {
            get { return _q; }
            set { _q = value; }
        }

        private int _emP;
        /// <summary>
        /// 电表有功电能
        /// </summary>
        public int EMP
        {
            get { return _emP; }
            set { _emP = value; }
        }

        private int _emQ;
        /// <summary>
        /// 电表无功电能
        /// </summary>
        public int EMQ
        {
            get { return _emQ; }
            set { _emQ = value; }
        }

        private byte _soc;
        /// <summary>
        /// SOC
        /// </summary>
        public byte SOC
        {
            get { return _soc; }
            set { _soc = value; }
        }

        private byte _faultcode;
        /// <summary>
        /// 故障码
        /// </summary>
        public byte FaultCode
        {
            get { return _faultcode; }
            set { _faultcode = value; }
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
            int start = 0;
            body[start] = this._qport;
            start += 1;
            body[start] = this._subscribeState;
            start += 1;
            body[start] = this._carportState;
            start += 1;
            body[start] = this._connectState;
            start += 1;
            body[start] = this._workingState;
            start += 1;
            byte[] temp = BitConverter.GetBytes(this._outputV);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._aphaseV);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._bphaseV);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._cphaseV);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._outputA);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._aphaseA);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._bphaseA);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._cphaseA);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            body[start] = this._outputRelayState;
            start += 1;
            body[start] = this._wpgWorkingState;
            start += 1;
            temp = BitConverter.GetBytes(this._rtTemp);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._p);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._q);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._emP);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            temp = BitConverter.GetBytes(this._emQ);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 4;
            body[start] = this._soc;
            start += 1;
            body[start] = this._faultcode;
            start += 1;
            temp = BitConverter.GetBytes(this._timestamp);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._qport = buffer[start];
            start += 1;
            this._subscribeState = buffer[start];
            start += 1;
            this._carportState = buffer[start];
            start += 1;
            this._connectState = buffer[start];
            start += 1;
            this._workingState = buffer[start];
            start += 1;
            this._outputV = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._aphaseV = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._bphaseV = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._cphaseV = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._outputA = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._aphaseA = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._bphaseA = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._cphaseA = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._outputRelayState = buffer[start];
            start += 1;
            this._wpgWorkingState = buffer[start];
            start += 1;
            this._rtTemp = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._p = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._q = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._emP = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._emQ = BitConverter.ToInt32(buffer, start);
            start += 4;
            this._soc = buffer[start];
            start += 1;
            this._faultcode = buffer[start];
            start += 1;
            this._timestamp = BitConverter.ToInt32(buffer, start);
            return this;
        }

        public UniversalData GetUniversalData()
        {
            UniversalData data = new UniversalData();


            return data;
        }
    }
}
