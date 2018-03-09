using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Redis
{
    using StackExchange.Redis;

    public class RedisPubSubServer : IDisposable
    {
        private bool _disposed = false;
        private ConnectionMultiplexer _redisClient;
        private ISubscriber subscriber;
        private RedisChannel[] _channel;
        private Action<RedisChannel, RedisValue> _onMessage;

        public RedisPubSubServer(ConnectionMultiplexer client, RedisChannel[] channel, Action<RedisChannel, RedisValue> onMessage)
        {
            _redisClient = client;
            _channel = channel;
            this._onMessage = onMessage;
        }

        public void Start()
        {
            subscriber = _redisClient.GetSubscriber();
            foreach (var item in _channel)
            {
                subscriber.Subscribe(item, _redisClient_SubscriptionReceived);
            }
        }

        private void _redisClient_SubscriptionReceived(RedisChannel sender, RedisValue e)
        {
            this._onMessage?.Invoke(sender, e);
        }

        public void Stop()
        {
            if (_redisClient != null)
            {
                subscriber.UnsubscribeAll();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                Stop();
                _redisClient?.Dispose();
                _redisClient = null;
                _disposed = true;
            }
        }
    }
}
