using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class StartChargingWithCardResultReplyPacket : PacketBase
    {
        public StartChargingWithCardResultReplyPacket() : base(PacketTypeEnum.StartChargingWithCardResultReply)
        {
            BodyLen = 1 + 11 + 8;
        }

        public StartChargingWithCardResultReplyPacket(PacketTypeEnum pte) : base(pte)
        {

        }

        private byte _qport;

        public byte QPort
        {
            get { return _qport; }
            set { _qport = value; }
        }

        /// <summary>
        /// 用户名，11位
        /// </summary>
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private long _transactionSN;

        public long TransactionSN
        {
            get { return _transactionSN; }
            set { _transactionSN = value; }
        }

        public override byte[] EncodeBody()
        {
            byte[] body = base.EncodeBody();
            int start = 0;
            body[start] = this._qport;
            start += 1;
            byte[] temp = EncodeHelper.GetBytes(this._userName);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 11;
            temp = BitConverter.GetBytes(this._transactionSN);
            Array.Copy(temp, 0, body, start, temp.Length);
            return body;
        }

        public override PacketBase DecodeBody(byte[] buffer)
        {
            base.DecodeBody(buffer);
            int start = 0;
            this._qport = buffer[start];
            start += 1;
            this._userName = EncodeHelper.GetString(buffer, start, 11);
            start += 11;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            return this;
        }
    }
}
