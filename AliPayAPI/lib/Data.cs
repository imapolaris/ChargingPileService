﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using Aop.Api.Util;

namespace AliPayAPI
{
    /// <summary>
    /// 支付宝支付协议接口数据类
    /// 在调用接口之前先填充各个字段的值，然后进行接口通信，
    /// 这样设计的好处是可扩展性强，用户可随意对协议进行更改而不用重新设计数据结构，
    /// 还可以随意组合出不同的协议数据包，不用为每个协议设计一个数据包结构
    /// </summary>
    public class AliPayData
    {
        public AliPayData()
        {

        }

        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /**
        * 设置某个字段的值
        * @param key 字段名
         * @param value 字段值
        */
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

        /**
        * 根据字段名获取某个字段的值
        * @param key 字段名
         * @return key对应的字段值
        */
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /**
         * 判断某个字段是否已设置
         * @param key 字段名
         * @return 若字段key已被设置，则返回true，否则返回false
         */
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        /**
        * @将Dictionary转成xml
        * @return 经转换得到的xml串
        * @throws AliPayException
        **/
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                Log.Error(this.GetType().ToString(), "AliPayData数据为空!");
                throw new AliPayException("AliPayData数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    Log.Error(this.GetType().ToString(), "AliPayData内部含有值为null的字段!");
                    throw new AliPayException("AliPayData内部含有值为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    Log.Error(this.GetType().ToString(), "AliPayData字段数据类型错误!");
                    throw new AliPayException("AliPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /**
        * @将xml转为AliPayData对象并返回对象内部的数据
        * @param string 待转换的xml串
        * @return 经转换得到的Dictionary
        * @throws AliPayException
        */
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Log.Error(this.GetType().ToString(), "将空的xml串转换为AliPayData不合法!");
                throw new AliPayException("将空的xml串转换为AliPayData不合法!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到AliPayData内部的数据中
            }
			
            try
            {
				//2015-06-29 错误是没有签名
				if(m_values["return_code"] != "SUCCESS")
				{
					return m_values;
				}
                CheckSign();//验证签名,不通过会抛异常
            }
            catch(AliPayException ex)
            {
                throw new AliPayException(ex.Message);
            }

            return m_values;
        }

        public void FromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.Error(this.GetType().ToString(), "将空的url串转换为AliPayData不合法");
                throw new AliPayException("将空的url串转换为AliPayData不合法");
            }

            var args = url.Split(new char[] { '&' });
            foreach (var arg in args)
            {
                var kv = arg.Split(new char[] { '=' });
                var key = HttpUtility.UrlDecode(kv[0]);
                var val = HttpUtility.UrlDecode(kv[1]);
                if (!string.IsNullOrEmpty(key) && !IsSet(key)
                    && key != "sign_type")
                {
                    m_values.Add(key, val);
                }
            }

            IDictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in m_values)
            {
                dic.Add(item.Key, item.Value?.ToString() ?? "");
            }
            var result = AlipaySignature.RSACheckV2(dic, AliPayConfig.ALIPAY_PUBLIC_KEY,
                AliPayConfig.CHARSET, AliPayConfig.SIGNTYPE, false);
            if (!result)
            {
                Log.Error(this.GetType().ToString(), "签名验证不通过");
                throw new AliPayException("签名验证不通过");
            }
        }

        /**
        * @Dictionary格式转化成url参数格式
        * @ return url格式串, 该串不包含sign字段值
        */
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    Log.Error(this.GetType().ToString(), "AliPayData内部含有值为null的字段!");
                    throw new AliPayException("AliPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Key != "sign_type" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }


        /**
        * @Dictionary格式化成Json
         * @return json串数据
        */
        public string ToJson()
        {
            string jsonStr = JsonMapper.ToJson(m_values);
            return jsonStr;
        }

        /**
        * @values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
        */
        public string ToPrintStr()
        {
            string str = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    Log.Error(this.GetType().ToString(), "AliPayData内部含有值为null的字段!");
                    throw new AliPayException("AliPayData内部含有值为null的字段!");
                }

                str += string.Format("{0}={1}<br>", pair.Key, pair.Value.ToString());
            }
            Log.Debug(this.GetType().ToString(), "Print in Web Page : " + str);
            return str;
        }

        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign()
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
               Log.Error(this.GetType().ToString(), "AliPayData签名存在但不合法!");
               throw new AliPayException("AliPayData签名存在但不合法!");
            }
            //如果设置了签名但是签名为空，则抛异常
            else if(GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                Log.Error(this.GetType().ToString(), "AliPayData签名存在但不合法!");
                throw new AliPayException("AliPayData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = "";//MakeSign();

            if (cal_sign == return_sign)
            {
                return true;
            }

            Log.Error(this.GetType().ToString(), "AliPayData签名验证错误!");
            throw new AliPayException("AliPayData签名验证错误!");
        }

        /**
        * @获取Dictionary
        */
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }
    }
}