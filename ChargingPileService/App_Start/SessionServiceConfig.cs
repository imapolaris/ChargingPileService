using ChargingPileService.Models;
using CPS.Infrastructure.MQ;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargingPileService
{
    using CSRedis;
    using CPS.Infrastructure.Redis;
    using System.Threading;
    using CPS.Infrastructure.Models;

    public class SessionServiceConfig
    {
        private static readonly string[] Channels = new string[] { ConfigHelper.Message_From_Tcp_Channel };
        private IMqManager MqManager { get; set; }
        private bool _registered = false;
        private const string SessionContainerKey = "SessionContainer";

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

                        using (var _client = RedisManager.GetClient())
                        {
                            string[] fields = _client.HKeys(SessionContainerKey);
                            List<string> delFields = new List<string>();
                            if (fields != null && fields.Count() > 0)
                            {
                                var sessions = _client.HMGet(SessionContainerKey, fields);
                                if (sessions != null && sessions.Length > 0)
                                {
                                    foreach (var item in sessions)
                                    {
                                        var session = JsonHelper.Deserialize<Session>(item);
                                        if (session.Outdated)
                                        {
                                            delFields.Add(session.Id);
                                        }
                                    }
                                }
                            }
                            if (delFields.Count > 0)
                            {
                                _client.HDelAsync(SessionContainerKey, delFields.ToArray());
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
            using (var _client = RedisManager.GetClient())
            {
                Session session = new Session(timeout);
                _client.FlushDb();
                bool success = _client.HSet(SessionContainerKey, session.Id, JsonHelper.Serialize(session));
                if (success)
                    return session;
                else
                    return null;
            }
        }

        public Session GetSession(string id)
        {
            using (var _client = RedisManager.GetClient())
            {
                string json = _client.HGet(SessionContainerKey, id);
                if (string.IsNullOrEmpty(json))
                    return null;
                else
                {
                    return JsonHelper.Deserialize<Session>(json);
                }
            }
        }

        public bool UpdateSession(Session session)
        {
            using (var _client = RedisManager.GetClient())
            {
                return _client.HSet(SessionContainerKey, session.Id, JsonHelper.Serialize(session));
            }
        }

        public bool RemoveOneSession(string id)
        {
            using (var _client = RedisManager.GetClient())
            {
                long count = _client.HDel(SessionContainerKey, id);
                return count >= 1;
            }
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
            //_client = RedisManager.GetClient();
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