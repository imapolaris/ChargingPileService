using ChargingPileService.Models;
using CPS.Infrastructure.MQ;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService
{
    using CPS.Infrastructure.Redis;
    using System.Threading;
    using CPS.Infrastructure.Models;
    using StackExchange.Redis;

    public class SessionServiceConfig
    {
        private static readonly RedisChannel[] Channels = new RedisChannel[] { ConfigHelper.Message_From_Tcp_Channel };
        private IMqManager MqManager { get; set; }
        private bool _registered = false;
        private const string SessionContainerKey = "SessionContainer";
        private ConnectionMultiplexer _redis = null;

        public void Register()
        {
            if (!_registered)
            {
                try
                {
                    MqManager = new MqManager_Redis(Channels);
                    MqManager.MessageReceived += MqManager_MessageReceived;
                    MqManager.Start();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                StartSessionStateDetection();

                _registered = true;
            }
        }

        private void MqManager_MessageReceived(string msg)
        {
            UniversalData data = new UniversalData();
            data.FromJson(msg);
            var id = data.GetStringValue("id");
            if (string.IsNullOrEmpty(id))
                return;
            Session session = new Session()
            {
                Id = id,
                Result = msg,
                IsCompleted = true
            };
            UpdateSession(session);
        }

        Thread ThreadSessionStateDetection;
        private bool stopSessionStateDetection = false;
        private int SessionStateDetectionInterval = 60 * 1000;

        /// <summary>
        /// 轮询会话状态
        /// </summary>
        private void StartSessionStateDetection()
        {
            ThreadSessionStateDetection = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (ThreadSessionStateDetection == null
                        || !ThreadSessionStateDetection.IsAlive
                        || stopSessionStateDetection)
                            break;

                        var db = _redis.GetDatabase();
                        var sessions = db.HashValues(SessionContainerKey);
                        if (sessions != null && sessions.Length > 0)
                        {
                            foreach (var item in sessions)
                            {
                                var session = JsonHelper.Deserialize<Session>(item);
                                if (session.Outdated)
                                {
                                    db.HashDelete(SessionContainerKey, session.Id);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    Thread.Sleep(SessionStateDetectionInterval);
                }
            })
            { IsBackground = true };
            ThreadSessionStateDetection.Start();
        }

        public Session StartOneSession(int timeout=10*1000)
        {
            var db = _redis.GetDatabase();
            Session session = new Session(timeout);
            bool success = db.HashSet(SessionContainerKey, session.Id, JsonHelper.Serialize(session));
            if (success)
                return session;
            else
                return null;
        }

        public Session GetSession(string id)
        {
            var db = _redis.GetDatabase();
            string json = db.HashGet(SessionContainerKey, id);
            if (string.IsNullOrEmpty(json))
                return null;
            else
            {
                return JsonHelper.Deserialize<Session>(json);
            }
        }

        public bool UpdateSession(Session session)
        {
            var db = _redis.GetDatabase();
            return db.HashSet(SessionContainerKey, session.Id, JsonHelper.Serialize(session));
        }

        public bool RemoveOneSession(string id)
        {
            var db = _redis.GetDatabase();
            return db.HashDelete(SessionContainerKey, id);
        }

        #region singleton
        public static SessionServiceConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SessionServiceConfig();
                }
                return _instance;
            }
        }
        private SessionServiceConfig()
        {
            _redis = RedisManager.GetClient();
        }
        private static SessionServiceConfig _instance;
        #endregion
    }

    public static class EntityExtension
    {
        public static bool RemoveMyself(this Session s)
        {
            return SessionServiceConfig.Instance.RemoveOneSession(s.Id);
        }

        public static bool IsSessionCompleted(this Session s)
        {
            var session = SessionServiceConfig.Instance.GetSession(s.Id);
            if (session == null)
                return false;
            return session.IsCompleted;
        }

        public static UniversalData GetSessionResult(this Session s)
        {
            var session = SessionServiceConfig.Instance.GetSession(s.Id);
            if (session == null)
                return null;
            else
            {
                return session.GetResult();
            }
        }
    }
}