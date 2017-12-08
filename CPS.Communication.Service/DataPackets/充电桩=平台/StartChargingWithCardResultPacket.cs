using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class StartChargingWithCardResultPacket : PacketBase
    {
        public StartChargingWithCardResultPacket() : base(PacketTypeEnum.StartChargingWithCardResult)
        {
            BodyLen = 1 + CardNoLen + 1 + 1 + 8 + 4;
        }

        private byte _qport;

        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }

        private string _cardNo;

        public string CardNo
        {
            get { return _cardNo; }
            set { _cardNo = value; }
        }

        private byte _cardState;
        /// <summary>
        /// 卡号状态
        /// </summary>
        public byte CardState
        {
            get { return _cardState; }
            set { _cardState = value; }
        }

        public CardStateTypeEnum CardStateEnum
        {
            get
            {
                return (CardStateTypeEnum)this._cardState;
            }
        }

        public string CardStateDesc
        {
            get
            {
                switch (this.CardStateEnum)
                {
                    case CardStateTypeEnum.NormalUser:
                        return "普通用户";
                    case CardStateTypeEnum.WhileListUser:
                        return "白名单用户";
                    case CardStateTypeEnum.BlackListUser:
                    default:
                        return "黑名单用户";
                }
            }
        }

        private byte _result;

        public byte Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public ResultTypeEnum ResultEnum
        {
            get
            {
                return (ResultTypeEnum)this._result;
            }
        }

        public string ResultString
        {
            get
            {
                switch (this.ResultEnum)
                {
                    case ResultTypeEnum.Succeed:
                        return "启动成功";
                    case ResultTypeEnum.Failed:
                    default:
                        return "启动失败";
                }
            }
        }

        private long _transactionSN;

        public long TransactionSN
        {
            get { return _transactionSN; }
            set { _transactionSN = value; }
        }

        private int _curElecMeter;
        /// <summary>
        /// 当前电表读数
        /// </summary>
        public int CurElecMeter
        {
            get { return _curElecMeter; }
            set { _curElecMeter = value; }
        }

        public double CurElecMeterVal
        {
            get
            {
                return this._curElecMeter * 0.01;
            }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = 0;
            body[start] = this._qport;
            start += 1;
            byte[] temp = EncodeHelper.GetBytes(this._cardNo);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += CardNoLen;
            body[start] = this._cardState;
            start += 1;
            body[start] = this._result;
            start += 1;
            temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            temp = BitConverter.GetBytes(this._curElecMeter);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._qport = buffer[start];
            start += 1;
            this._cardNo = EncodeHelper.GetString(buffer, start, CardNoLen);
            start += CardNoLen;
            this._cardState = buffer[start];
            start += 1;
            this._result = buffer[start];
            start += 1;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._curElecMeter = BitConverter.ToInt32(buffer, start);
            return this;
        }
    }
}
