using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jiguang.JPush;
using Jiguang.JPush.Model;
using CPS.Infrastructure.Utils;
using System.Reflection;

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
            AppKey = ConfigHelper.GetValue(typeof(PushMessage).Assembly, "appKey");
            MasterSecret = ConfigHelper.GetValue(typeof(PushMessage).Assembly, "masterSecret");
        }

        private PushMessage()
        {
            JPClient = new JPushClient(AppKey, MasterSecret);
        }

        public async Task<HttpResponse> PushNotificationAsync()
        {
            PushPayload pushPayload = new PushPayload()
            {
                Platform = "android",
                Audience = "all",
                Notification = new Notification()
                {
                    Alert = "hello jpush",
                    Android = new Android()
                    {
                        Alert = "android alert",
                        Title = "title"
                    },
                    IOS = new IOS()
                    {
                        Alert = "ios alert",
                        Badge = "+1"
                    }
                }
            };
            var response = await JPClient.SendPushAsync(pushPayload);
            Console.WriteLine(response.Content);

            return response;
        }

        public HttpResponse PushNotification()
        {
            var task = Task.Run(() => PushNotificationAsync());
            task.Wait();
            return task.Result;
        }

        public async Task<HttpResponse> PushCustomMessageAsync()
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
                        ["key1"] = "value1"
                    }
                }
            };
            var response = await JPClient.SendPushAsync(pushPayload);
            Console.WriteLine(response.Content);

            return response;
        }

        public HttpResponse PushCustomMessage()
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
    }
}
