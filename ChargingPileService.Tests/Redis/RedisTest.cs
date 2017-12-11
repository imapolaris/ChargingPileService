using Microsoft.VisualStudio.TestTools.UnitTesting;
using CPS.Infrastructure.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CPS.Infrastructure.Redis.Tests
{
    [TestClass()]
    public class RedisTest
    {
        private const string SerialNumber = "1234567890aAbcDE";
        private const string Channel = "SOARING";
        private const string Message = "Hello alex!";

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod()]
        public void RedisOperatorTest()
        {
            var oper = new RedisOperator(SerialNumber);
            Assert.IsNotNull(oper);
        }

        [TestMethod()]
        public void GetTagValuesTest()
        {
            var oper = new RedisOperator(SerialNumber);
            var result = oper.GetTagValues(new List<string> { SerialNumber });
            Assert.IsTrue(result != null && result.Count > 0);
        }

        [TestMethod()]
        public void SetTagValuesTest()
        {
            var oper = new RedisOperator(SerialNumber);
            oper.SetTagValues(new Dictionary<string, string>()
            {
                { SerialNumber, "123" },
            });
        }

        [TestMethod()]
        public void PublishTest()
        {
            var oper = new RedisOperator(SerialNumber);
            var result = oper.Publish(Channel, Message);
            Assert.IsTrue(result >= 0);
        }

        [TestMethod()]
        public void RedisPubSubServerTest()
        {
            Task.Run(() =>
            {
                var pub = RedisManager.GetPubServer((s1, s2) =>
                {
                    Assert.AreEqual(Message, s2);
                }, new string[] { Channel });
                {
                    pub.Start();
                }

                return pub;
            }).ContinueWith((pub)=>
            {
                //Thread.Sleep(1000);
                //pub.Stop();
            });

            Thread.Sleep(1000);

            Task.Run(() =>
            {
                var oper = new RedisOperator(SerialNumber);
                var result = oper.Publish(Channel, Message);
                
                return result;
            }).ContinueWith((result) =>
            {
                Assert.AreEqual(1, result);
            });
        }
    }
}