using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public class SendCompletedEventArgs : EventArgs
    {
        private int _len;

        public int Len
        {
            get { return _len; }
            set { _len = value; }
        }

        public SendCompletedEventArgs()
        {
        }

        public SendCompletedEventArgs(int len)
        {
            this._len = len;
        }
    }
}
