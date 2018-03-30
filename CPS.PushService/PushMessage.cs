using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jiguang.JPush;
using Jiguang.JPush.Model;
using CPS.Infrastructure.Utils;
using System.Reflection;
using CPS.Infrastructure.Enums;
using System.Collections;

namespace CPS.PushService
{
    /// <summary>
    /// 推送消息
    /// </summary>
    public class PushMessage
    {
        private static readonly string AppKey = "";
        private static readonly string MasterSecret = "";
        private readonly JPushClient JPClient;

        static PushMessage _instance;
        public static PushMessage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PushMessage();
                return _instance;
            }
        }

        static PushMessage()
        {
            //AppKey = ConfigHelper.GetValue(typeof(PushMessage).Assembly, "appKey");
            //MasterSecret = ConfigHelper.GetValue(typeof(PushMessage).Assembly, "masterSecret");

            AppKey = ConfigHelper.PushAppKey;
            MasterSecret = ConfigHelper.PushMasterSecret;
        }

        private PushMessage()
        {
            JPClient = new JPushClient(AppKey, MasterSecret);
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
    }
}
