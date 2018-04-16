using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.MessageSystem
{
    using Aliyun.Acs.Core;
    using Aliyun.Acs.Core.Exceptions;
    using Aliyun.Acs.Core.Profile;
    using Aliyun.Acs.Dysmsapi.Model.V20170525;
    using Infrastructure.Utils;
    using RabbitMQ.Client.Events;
    using Infrastructure.Models;

    /// <summary>
    /// 短信服务
    /// </summary>
    public class SmsService : ServiceBase
    {
        string accessKeyId, accessKeySecret, regionId, signName, templateCode, templateParam;

        protected override string QueueName { get; set; } = "CPS_MQ_SMS";
        private const int TIMEOUT = 5 * 60; // 5分钟

        public SmsService()
        {
            MessageConfiguration messageConfig = MessageConfiguration.GetConfig();
            accessKeyId = messageConfig.AliAccessKeyId;//你的accessKeyId
            accessKeySecret = messageConfig.AliAccessKeySecret;//你的accessKeySecret
            regionId = messageConfig.AliRegionId;
            signName = messageConfig.AliSignName;
            templateCode = messageConfig.AliTemplateCode;
            templateParam = messageConfig.AliTemplateParam;
        }

        #region 【消息队列】

        public override void StartMQConsumer()
        {
            // 声明一个队列
            channel.QueueDeclare(queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // 每次只接收1条消息进行处理
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Logger.Info("打开一个短信消息消费客户端...");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        var body = ea.Body;
                        var json = EncodeHelper.GetString(body);
                        if (string.IsNullOrEmpty(json))
                            throw new ArgumentNullException("收到空的短信消息...");
                        UniversalData data = new UniversalData();
                        data.FromJson(json);

                        // 检查是否已超时
                        if (!data.IsSet("timestamp") || DateHelper.IsTimeout(data.GetIntValue("timestamp"), TIMEOUT))
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
                            return;
                        }

                        if (!data.IsSet("telephone"))
                            throw new ArgumentException("没有必须的telephone参数");
                        var telephone = data.GetStringValue("telephone");
                        var result = SendSMS(telephone);
                        if (result)
                        {
                            Logger.Info($"向{telephone}发送短信成功！");
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        else
                        {
                            Logger.Info($"向{telephone}发送短信失败！");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                });
            };
            consumer.Registered += (model, ea) =>
            {
                Task.Run(() =>
                {
                    Logger.Info("短信消息消费客户端已注册.");
                });
            };
            consumer.Unregistered += (model, ea) =>
            {
                Task.Run(() =>
                {
                    Logger.Info("短信消息消费客户端已取消注册.");
                });
            };
            consumer.Shutdown += (model, ea) =>
            {
                Task.Run(() =>
                {
                    Logger.Info("短信消息消费客户端已关闭.");
                });
            };

            // 打开一个消费者
            channel.BasicConsume(queue: QueueName, autoAck:false, consumerTag: "", noLocal: false, exclusive: false, arguments: null, consumer: consumer);

            Logger.Info("打开短信消息消费客户端完成.");
        }

        public override void StopMQConsumer()
        {
            Logger.Info("一个短信消息消费客户端即将关闭.");
            this.Dispose();
        }

        #endregion

        public bool SendSMS(string phoneNumber)
        {
            string product = "Dysmsapi";//短信API产品名称
            string domain = "dysmsapi.aliyuncs.com";//短信API产品域名
            

            IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKeyId, accessKeySecret);
            //IAcsClient client = new DefaultAcsClient(profile);
            // SingleSendSmsRequest request = new SingleSendSmsRequest();

            DefaultProfile.AddEndpoint(regionId, regionId, product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            var vcode = Number();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为20个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = phoneNumber;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = signName;
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = templateCode;
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = "{\"" + templateParam + "\":\"" + vcode + "\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                //request.OutId = "mayew.com";
                //请求失败这里会抛ClientException异常
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                return ReturnStatus(sendSmsResponse, phoneNumber, vcode);
            }
            catch (ServerException e)
            {
                Logger.Error(e.Message);
            }
            catch (ClientException e)
            {
                Logger.Error(e.Message);
            }

            return false;
        }

        private bool ReturnStatus(SendSmsResponse response, string phoneNumber, string vcode)
        {
            if (response.Code == "OK")
            {
                SmsManager.Instance.AppendVCode(phoneNumber, vcode);
                return true;
            }
            return false;
        }

        private string Number(int Length = 6)
        {
            string result = "";
            Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
