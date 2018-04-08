﻿using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.DataPackets
{
    public class StartChargingWithCardPacket : PacketBase
    {
        public StartChargingWithCardPacket() : base(PacketTypeEnum.StartChargingWithCard)
        {
            BodyLen = 1 + 11 + 20 + 8;
        }

        public StartChargingWithCardPacket(PacketTypeEnum pte) : base(pte) { }

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

        /// <summary>
        /// 密码，20位
        /// </summary>
        private string _pwd;
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
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
            temp = EncodeHelper.GetBytes(this._pwd);
            Array.Copy(temp, 0, body, start, temp.Length);
            start += 20;
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
            this._pwd = EncodeHelper.GetString(buffer, start, 20);
            start += 20;
            this._transactionSN = BitConverter.ToInt64(buffer, start);
            return this;
        }
    }
}
