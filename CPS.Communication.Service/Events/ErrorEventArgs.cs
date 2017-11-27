using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Communication.Service.Events
{
    public class ErrorEventArgs : EventArgs
    {
        private string _msg;
        private int _code = 0;
        private ErrorTypes _eType;
        private Exception _inner;

        public string ErrorMessage { get { return _msg; } }
        public int SocketErrorCode { get { return _code; } }
        public Exception InnerException { get { return _inner; } }
        public ErrorTypes ErrorType { get { return _eType; } }

        public ErrorEventArgs(string errorMsg, ErrorTypes errorType)
        {
            _msg = errorMsg;
            _eType = errorType;
        }

        public ErrorEventArgs(string errorMsg, ErrorTypes errorType, int errorCode, Exception innerEx) : this(errorMsg, errorType)
        {
            _inner = innerEx;
            _code = errorCode;
        }

        public override string ToString()
        {
            string msg = $"----错误类型：{this._eType.ToString()}, 错误信息：{this._msg}, 错误编码：{this._code}\n";
            if (this._inner != null)
            {
                msg += $"堆栈信息：\n";
                msg += PrintInnerException(this._inner);
            }
            return msg;
        }

        private string PrintInnerException(Exception ex, string msg="")
        {
            if (ex != null)
            {
                msg += ex.Message + "\n";
                return PrintInnerException(ex.InnerException, msg);
            }
            else
            {
                return msg;
            }
        }
    }
}
