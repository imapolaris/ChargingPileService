using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core;

namespace ChargingPileService.Tests
{
    [TestClass]
    public class SendMessageTest
    {
        // ali accesskey
        const string MyAccessKeyId = "LTAI8WvkvOREy741";
        const string MyAccessKeySecret = "GEe7u89wEHwP89tJqBsLVv5vzRl5Ly";

        [TestMethod]
        public void TestSendMessage()
        {
            SendMessage("13269734774");
        }

        private void SendMessage(string phoneNumber)
        {
            String product = "Dysmsapi";//短信API产品名称
            String domain = "dysmsapi.aliyuncs.com";//短信API产品域名
            String accessKeyId = MyAccessKeyId;//你的accessKeyId
            String accessKeySecret = MyAccessKeySecret;//你的accessKeySecret

            IClientProfile profile = DefaultProfile.GetProfile("cn-beijing", accessKeyId, accessKeySecret);
            //IAcsClient client = new DefaultAcsClient(profile);
            // SingleSendSmsRequest request = new SingleSendSmsRequest();

            DefaultProfile.AddEndpoint("cn-beijing", "cn-beijing", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为20个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = phoneNumber;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "嘛哟科技";
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = "SMS_94190001";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = "{\"code\":\"123456\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                //request.OutId = "mayew.com";
                //请求失败这里会抛ClientException异常
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);

                System.Console.WriteLine(sendSmsResponse.Message);
            }
            catch (ServerException e)
            {
                System.Console.WriteLine("Hello World!");
            }
            catch (ClientException e)
            {
                System.Console.WriteLine("Hello World!");
            }
        }
    }
}
