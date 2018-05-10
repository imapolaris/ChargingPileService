using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ChargingPileService.Models
{
    using CPS.Infrastructure.Models;

    public class Session
    {
        private const int DefaultTimeout = 10 * 1000;

        public string Id { get; set; }
        public bool IsCompleted { get; set; }
        public string Result { get; set; }

        public DateTime StartDate { get; set; }
        /// <summary>
        /// 超时时间（单位：毫秒）
        /// </summary>
        public int Timeout { get; private set; }// = DefaultTimeout;
        public bool Outdated
        {
            get
            {
                return (DateTime.Now - StartDate).TotalMilliseconds > Timeout;
            }
        }

        /// <summary>
        /// 消息重发次数
        /// </summary>
        public int RetryTimes { get; set; } = 0;

        public Session(int timeout = DefaultTimeout)
        {
            Id = Guid.NewGuid().ToString();
            Timeout = timeout;
            StartDate = DateTime.Now;
        }

        public async Task<bool> WaitSessionCompleted()
        {
            await Task.Run(() =>
            {
                while (!Outdated && !IsCompleted)
                {
                    IsCompleted = this.IsSessionCompleted();
                    Task.Delay(5);
                }
            });

            if (IsCompleted)
                return true;
            else
            {
                if (RetryTimes <= 0)
                    return false;
                else
                {
                    StartDate = DateTime.Now;
                    RetryTimes--;
                    return await WaitSessionCompleted();
                }
            }
        }

        public UniversalData GetResult()
        {
            UniversalData data = new UniversalData();
            data.FromJson(this.Result);
            return data;
        }
    }
}