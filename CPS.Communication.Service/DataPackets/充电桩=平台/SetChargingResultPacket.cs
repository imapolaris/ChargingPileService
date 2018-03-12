using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPS.Infrastructure.Models;
using CPS.Infrastructure.Enums;

namespace CPS.Communication.Service.DataPackets
{
    public class SetChargingResultPacket : OperPacketBase, IUniversal
    {
        public SetChargingResultPacket() : base(PacketTypeEnum.SetChargingResult)
        {
            BodyLen = OperPacketBodyLen + 8 + 1 + 1 + 1 + 1 + 1;
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

        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }

        private byte _action;
        /// <summary>
        /// 操作
        /// </summary>
        public byte Action
        {
            get { return _action; }
            set { _action = value; }
        }

        private byte _result;
        /// <summary>
        /// 启停结果
        /// </summary>
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
                        return "成功";
                    case ResultTypeEnum.Failed:
                    default:
                        return "失败";
                }
            }
        }

        private byte _failReason;

        public byte FailReason
        {
            get { return _failReason; }
            set { _failReason = value; }
        }

        public FailReasonTypeEnum FailReasonEnum
        {
            get
            {
                return (FailReasonTypeEnum)this._failReason;
            }
        }

        public string FailReasonString
        {
            get
            {
                switch (this.FailReasonEnum)
                {
                    case FailReasonTypeEnum.Normal:
                        return "正常";
                    case FailReasonTypeEnum.TurnedOn:
                        return "已经开机";
                    case FailReasonTypeEnum.NotStandby:
                        return "不是待机状态";
                    case FailReasonTypeEnum.NotConnected:
                        return "枪未连接";
                    case FailReasonTypeEnum.Others:
                    default:
                        return "其他错误";
                }
            }
        }

        private byte _soc;

        public byte SOC
        {
            get { return _soc; }
            set { _soc = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = OperPacketBodyLen;
            byte[] temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 8;
            body[start] = this._qport;
            start += 1;
            body[start] = this._action;
            start += 1;
            body[start] = this._result;
            start += 1;
            body[start] = this._failReason;
            start += 1;
            body[start] = this._soc;
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = OperPacketBodyLen;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            start += 8;
            this._qport = buffer[start];
            start += 1;
            this._action = buffer[start];
            start += 1;
            this._result = buffer[start];
            start += 1;
            this._failReason = buffer[start];
            start += 1;
            this._soc = buffer[start];
            return this;
        }

        public UniversalData GetUniversalData()
        {
            UniversalData data = new UniversalData();
            data.SetValue("transSn", this._transactionSN);
            data.SetValue("port", this._qport);
            data.SetValue("oper", this._action);
            data.SetValue("result", this._result);
            data.SetValue("failReason", this._failReason);
            data.SetValue("soc", this._soc);

            return data;
        }
    }
}
