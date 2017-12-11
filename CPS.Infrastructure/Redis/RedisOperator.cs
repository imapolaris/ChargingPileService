using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPS.Infrastructure.Redis
{
    public class RedisOperator : RedisBase
    {
        private readonly string _hashKey;
        private readonly string _channelKey;

        public RedisOperator(string sn) : base()
        {
            this._hashKey = $"{sn}";
            this._channelKey = $"{sn}";
        }

        public Dictionary<string, string> GetTagValues(List<string> tagNames)
        {
            var result = new Dictionary<string, string>();

            if (tagNames?.Count > 0)
            {
                var vs = this.Client.HMGet(this._hashKey, tagNames.ToArray()) ?? new string[] { };
                for (int i = 0; i < tagNames.Count; i++)
                {
                    if (i < vs.Length)
                    {
                        result.Add(tagNames[i], vs[i]);
                    }
                    else
                    {
                        result.Add(tagNames[i], null);
                    }
                }
            }
            else
            {
                result = this.Client.HGetAll(this._hashKey);
            }
            return result;
        }

        public void SetTagValues(Dictionary<string, string> list)
        {
            this.Client.HMSet(this._hashKey, list);
        }

        public long Publish(string channel,string msg)
        {
            return this.Client.Publish($"{channel}", msg);
        }
    }
}
