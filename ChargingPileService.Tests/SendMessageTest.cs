using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.Text;

namespace ChargingPileService.Tests
{
    [TestClass]
    public class SendMessageTest
    {
        [TestMethod]
        public async Task TestSendMessage()
        {
            string phoneNumbers = "13269734774";
            var url = "http://192.168.0.201/ChargingPileService/api/messages?phoneNumber=" + phoneNumbers;
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 2000;
                request.ContentLength = 0;
                await request.GetResponseAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
