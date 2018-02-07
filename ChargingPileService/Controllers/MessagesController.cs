using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using ChargingPileService.Models;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    public class MessagesController : OperatorBase
    {
        // ali accesskey
        private static readonly string MyAccessKeyId = ConfigHelper.AliAccessKeyId;
        private static readonly string MyAccessKeySecret = ConfigHelper.AliAccessKeySecret;
        private static readonly string MyRegionId = ConfigHelper.AliRegionId;
        private static readonly string MySignName = ConfigHelper.AliSignName;
        private static readonly string MyTemplateCode = ConfigHelper.AliTemplateCode;
        private static readonly string MyTemplateParam = ConfigHelper.AliTemplateParam;

        [HttpGet]
        public IHttpActionResult SendMessage(string phoneNumber)
        {
            String product = "Dysmsapi";//短信API产品名称
            String domain = "dysmsapi.aliyuncs.com";//短信API产品域名
            String accessKeyId = MyAccessKeyId;//你的accessKeyId
            String accessKeySecret = MyAccessKeySecret;//你的accessKeySecret

            IClientProfile profile = DefaultProfile.GetProfile(MyRegionId, accessKeyId, accessKeySecret);
            //IAcsClient client = new DefaultAcsClient(profile);
            // SingleSendSmsRequest request = new SingleSendSmsRequest();

            DefaultProfile.AddEndpoint(MyRegionId, MyRegionId, product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            var vcode = Number();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为20个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = phoneNumber;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = MySignName;
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = MyTemplateCode;
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = "{\""+ MyTemplateParam + "\":\"" + vcode + "\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                //request.OutId = "mayew.com";
                //请求失败这里会抛ClientException异常
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                var status = ReturnStatus(sendSmsResponse, phoneNumber, vcode);
                if (status == true)
                {
                    return Ok(new SimpleResult(true, "验证码发送成功！"));
                }
                else
                {
                    Logger.Error(sendSmsResponse.Message);
                    return Ok(new SimpleResult(false, "验证码发送失败！"));
                }
            }
            catch (ServerException e)
            {
                Logger.Error(e.Message);
                return Content<string>(HttpStatusCode.InternalServerError, "server error");
            }
            catch (ClientException e)
            {
                Logger.Error(e.Message);
                return Content<string>(HttpStatusCode.BadRequest, "bad request");
            }
        }

        [NonAction]
        private bool ReturnStatus(SendSmsResponse response, string phoneNumber, string vcode)
        {
            if (response.Code == "OK")
            {
                SmsServiceConfig.Instance.AppendVCode(phoneNumber, vcode);
                return true;
            }
            return false;
        }

        [NonAction]
        private string Number(int Length=6)
        {
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
    }
}
