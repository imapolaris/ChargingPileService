using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CPS.Infrastructure.Utils;
using System.Diagnostics;

namespace ChargingPileService.Tests
{
    /// <summary>
    /// UtilsTest 的摘要说明
    /// </summary>
    [TestClass]
    public class UtilsTest
    {
        public UtilsTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void AESEncrypt()
        {
            string result = EncryptHelper.Encrypt("hello world!");
            Debug.WriteLine(result);
        }

        [TestMethod]
        public void AESDecrypt()
        {
            string text = "9lJ76/WscXWGkIfTGnIByg=="; 
            string result = EncryptHelper.Decrypt(text);
            Assert.AreEqual("hello world!", result, false);
        }

        [TestMethod]
        public void AESEncrypt2()
        {
            byte[] result = EncryptHelper.Encrypt(EncodeHelper.GetBytes("hello, world!"));
            string temp = "";
            foreach (var item in result)
            {
                temp += "0x" + item.ToString("x") + ",";
            }
            Debug.WriteLine(temp);
        }

        [TestMethod]
        public void AESDecrypt2()
        {
            byte[] buffer = new byte[] { 0x1d, 0x6a, 0xc2, 0xbc, 0x92, 0x17, 0x6b, 0x9c, 0x55, 0xd4, 0x2b, 0xf1, 0x1c, 0x5b, 0x87, 0x2d, 0xed, 0x1b, 0xf2, 0xd0, 0x6, 0x9e, 0xe7, 0x9e, 0x16, 0x20, 0x94, 0x6b, 0xe, 0x37, 0x35, 0xe4 };
            byte[] result = EncryptHelper.Decrypt(buffer);
            string temp = EncodeHelper.GetString(result);
            Assert.AreEqual("hello, world!", temp, true);
        }
    }
}
