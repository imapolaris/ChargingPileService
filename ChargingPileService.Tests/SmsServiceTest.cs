using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargingPileService.Tests
{
    [TestClass]
    public class SmsServiceTest
    {
        SmsServiceConfig instance = SmsServiceConfig.Instance;

        [TestMethod]
        public void AppendVCode()
        {
            instance.AppendVCode("13269734774", "1234");
        }

        [TestMethod]
        public void ValidateVCode()
        {
            Assert.IsTrue(instance.ValidateVCode("13269734774", "1234"));
            Assert.IsFalse(instance.ValidateVCode("13269734774", "123"));
        }

        [TestMethod]
        public void RunClear()
        {
            instance.RunClear();
        }
    }
}
