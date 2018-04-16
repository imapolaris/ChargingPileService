using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace CPS.MessageSystem
{
    using Jiguang.JPush;
    using Jiguang.JPush.Model;
    using CPS.Infrastructure.Utils;
    using CPS.Infrastructure.Enums;
    using RabbitMQ.Client.Events;
    using Infrastructure.Models;

    /// <summary>
    /// 通知消息
    /// </summary>
    public class NotificationService : ServiceBase
    {
        string AppKey, MasterSecret;
        private readonly JPushClient JPClient;

        protected override string QueueName { get; set; } = "CPS_MQ_Notification";
        private const int TIMEOUT = Int32.MaxValue;

        public NotificationService()
        {
            var config = MessageConfiguration.GetConfig();
            AppKey = config.JGPushAppKey;
            MasterSecret = config.JGPushMasterSecret;
            JPClient = new JPushClient(AppKey, MasterSecret);
        }

        public override void StartMQConsumer()
        {
            channel.QueueDeclare(queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Logger.Info("打开一个通知消息消费客户端...");

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
                            throw new ArgumentNullException("通知消息参数为空...");
                        UniversalData data = new UniversalData();
                        data.FromJson(json);

                        // 检查推送消息是否超时
                        /*
                        if (!data.IsSet("timestamp") || DateHelper.IsTimeout(data.GetIntValue("timestamp"), TIMEOUT))
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
                            Logger.Info("通知消息超时！");
                            return;
                        }
                        */

                        if (!data.IsSet("platform") || !data.IsSet("content"))
                            throw new ArgumentException("缺少必须参数...");
                        var platform = (PlatformTypeEnum)data.GetIntValue("platform");
                        var content = data.GetStringValue("content");
                        var title = ""; // EV堡???
                        if (data.IsSet("title"))
                            title = data.GetStringValue("title");
                        Dictionary<string, object> extras = null;
                        if (data.IsSet("extras"))
                            extras = JsonHelper.Deserialize<Dictionary<string, object>>(data.GetStringValue("extras"));
                        var result = PushNotification(platform, title, content, extras);
                        if (result)
                        {
                            Logger.Info($"推送消息{title}：{content}成功！");
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        else
                        {
                            Logger.Info($"推送消息{title}：{content}失败！");
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
            channel.BasicConsume(queue: QueueName, autoAck: false, consumerTag: "", noLocal: false, exclusive: false, arguments: null, consumer: consumer);

            Logger.Info("打开通知消息消费客户端完成.");
        }

        public override void StopMQConsumer()
        {
            Logger.Info("一个通知消息消费客户端即将关闭.");
            this.Dispose();
        }

        public async Task<bool> PushNotificationAsync(PlatformTypeEnum platform, string title, string content, Dictionary<string, object> extras=null)
        {
            PushPayload pushPayload = new PushPayload()
            {
                Platform = GetPlatform(platform),
                Audience = "all",
                Notification = new Notification()
                {
                    //Alert = "hello jpush",
                    Android = new Android()
                    {
                        Alert = content,
                        Title = title,
                        Extras = extras,
                    },
                    IOS = new IOS()
                    {
                        Alert = content,
                        Badge = "+1"
                    }
                }
            };
            var response = await JPClient.SendPushAsync(pushPayload);
            Logger.Info(response.Content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logger.Info($"通知推送成功，内容：{content}");
                return true;
            }
            else
            {
                Logger.Info($"通知推送失败，原因：{StatusDesc((int)response.StatusCode)}");
                return false;
            }
        }

        public bool PushNotification(PlatformTypeEnum platform, string title, string content, Dictionary<string, object> extras=null)
        {
            var task = Task.Run(() => PushNotificationAsync(platform, title, content, extras));
            task.Wait();
            return task.Result;
        }

        public async Task<bool> PushCustomMessageAsync()
        {
            PushPayload pushPayload = new PushPayload()
            {
                Platform = "android",
                Audience = "all",
                Message = new Message()
                {
                    Title = "message title",
                    Content = "message content",
                    Extras = new Dictionary<string, string>()
                    {
                        ["url"] = "http://www.baidu.com"
                    }
                }
            };
            var response = await JPClient.SendPushAsync(pushPayload);
            Console.WriteLine(response.Content);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public bool PushCustomMessage()
        {
            var task = Task.Run(() => PushCustomMessageAsync());
            task.Wait();
            return task.Result;
        }

        public async Task<HttpResponse> PushRichMediaAsync()
        {
            await Task.Delay(3000);
            return null;
        }

        public HttpResponse PushRichMedia()
        {
            var task = Task.Run(() => PushRichMediaAsync());
            task.Wait();
            return task.Result;
        }

        private string GetPlatform(PlatformTypeEnum platform)
        {
            switch (platform)
            {
                case PlatformTypeEnum.All:
                    return "all";
                case PlatformTypeEnum.Android:
                    return "android";
                case PlatformTypeEnum.IOS:
                    return "ios";
                default:
                    return "all";
            }
        }

        private string StatusDesc(int statusCode)
        {
            switch (statusCode)
            {
                case 200:
                    return "OK";
                case 400:
                    return "错误的请求";
                case 401:
                    return "未验证";
                case 403:
                    return "被拒绝";
                case 404:
                    return "无法找到";
                case 405:
                    return "请求方法不合适";
                case 410:
                    return "已下线";
                case 429:
                    return "过多的请求";
                case 500:
                    return "内部服务错误";
                case 502:
                    return "无效代理";
                case 503:
                    return "服务暂时失效";
                case 504:
                    return "代理超时";
                default:
                    return "未知";
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
