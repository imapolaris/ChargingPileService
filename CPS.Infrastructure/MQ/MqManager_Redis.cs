using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CPS.Infrastructure.MQ
{
    using CPS.Infrastructure.Redis;
    using CPS.Infrastructure.Utils;
    using CSRedis;
    using Models;
    using System.Threading;

    public delegate void MessageReceivedHandler(string msg);

    public class MqManager_Redis : IDisposable, IMqManager
    {
        protected RedisPubSubServer PubSubServer { get; set; }
        private RedisClient _client = RedisManager.GetClient();


        public MqManager_Redis(string[] channel)
        {
            PubSubServer = new RedisPubSubServer(_client, channel, ReceiveMsg);
        }

        public void Start()
        {
            PubSubServer?.Start();
        }

        public void Stop()
        {
            PubSubServer?.Stop();
        }

        private void ReceiveMsg(string channel, string msg)
        {
            OnMessageReceived(msg);
        }

        #region 【Events】

        public event MessageReceivedHandler MessageReceived;

        private void OnMessageReceived(string msg)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                new Thread(() =>
                {
                    handler(msg);
                })
                { IsBackground = true }
                .Start();
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                PubSubServer?.Stop();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}