using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Diagnostics;

namespace ChargingPileService.Tests
{
    [TestClass]
    public class PayTest
    {
        [TestMethod]
        async public void Alipay()
        {
            string url = @"http://192.168.0.201/ChargingPileService/api/wallet/alipay";

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 2000;
                request.ContentLength = 0;
                await request.GetResponseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        async public void Wxpay()
        {
            string url = @"http://192.168.0.201/ChargingPileService/api/wallet/wxpay";

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 2000;
                request.ContentLength = 0;
                await request.GetResponseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
