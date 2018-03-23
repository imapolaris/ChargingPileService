using System;
using System.Collections.Generic;
using System.Web;

namespace AliPayAPI
{
    public class AliPayException : Exception 
    {
        public AliPayException(string msg) : base(msg) 
        {

        }
     }
}