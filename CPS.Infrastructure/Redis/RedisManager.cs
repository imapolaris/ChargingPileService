using CSRedis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Redis
{
    public class RedisManager
    {
        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private static RedisConfiguration _redisConfig = RedisConfiguration.GetConfig();
        private static string _pwd;
        private static RedisConnectionPool _prcm;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisManager()
        {
            CreateManager();
        }

        /// <summary>
        /// 创建链接池管理对象
        /// IP地址中可以加入auth验证   password@ip:port
        /// </summary>
        private static void CreateManager()
        {
            var endPoint = _redisConfig.AnalyseHost(ref _pwd);
            _prcm = new RedisConnectionPool(endPoint, _redisConfig.MaxPoolSize);
        }

        public static void Get()
        {
            ConnectionMultiplexer.Connect(new ConfigurationOptions()
            {

            });
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static RedisClient GetClient()
        {
            var p = "";
            var endPoint = _redisConfig.AnalyseHost(ref p);
            var client = new RedisClient(endPoint);
            if (!string.IsNullOrWhiteSpace(p))
                client.Auth(_pwd);
            return client;
        }

        private static RedisClient GetRpcClient()
        {
            if (_prcm == null)
                CreateManager();
            var client = _prcm.GetClient();
            if (client != null
                && !string.IsNullOrWhiteSpace(_pwd))
                client.Auth(_pwd);
            return client;
        }
        public static RedisPubSubServer GetPubServer(Action<string, string> OnMessage, params string[] channel)
        {
            var client = GetRpcClient();
            if (client == null)
            {
                //LoggerMng.Log.Error("连接池已经消耗至尽");
                throw new Exception("连接池已经消耗至尽");
            }

            return new RedisPubSubServer(client, channel, OnMessage);
        }
    }
}
