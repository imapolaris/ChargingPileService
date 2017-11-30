using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CPS.Entities
{
    public class Message : EntityBase
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public T GetValueByKey<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key不能为空");

            if (string.IsNullOrEmpty(this.extras))
                throw new ArgumentNullException("附加字段为空，无法查找");

            JObject jobj = new JObject(this.extras);
            JToken jt = jobj.GetValue(key);
            if (jt == null)
                throw new KeyNotFoundException("没有找到key对应的值");
            return jt.Value<T>();
        }

        private string extras;

        public string Extras
        {
            get { return extras; }
            set { extras = value; }
        }

    }
}
