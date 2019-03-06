using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using CCShop.Log4Net;

namespace CCShop.Service.WeChat
{
    public class WeChatPayData
    {
        public WeChatPayData()
        {

        }

        [DataMember]
        public string timeStamp { get; set; }

        [DataMember]
        public string nonceStr { get; set; }

        [DataMember]
        public string package { get; set; }

        [DataMember]
        public string signType { get; set; }

        [DataMember]
        public string paySign { get; set; }

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
        * @throws WxPayException
        **/
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                throw new Exception("WxPayData数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    throw new Exception("WxPayData内部含有值为null的字段!");
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
                    throw new Exception("WxPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /**
        * @将xml转为WxPayData对象并返回对象内部的数据
        * @param string 待转换的xml串
        * @return 经转换得到的Dictionary
        * @throws WxPayException
        */
        public SortedDictionary<string, object> LoadXml(string xml)
        {
            m_values = fromXML(xml);

            string trade_type = "";
            foreach (var item in m_values)
            {
                if (item.Key == "trade_type")
                {
                    trade_type = m_values["trade_type"].ToString();
                }
            }

            CheckSign(trade_type);//验证签名,不通过会抛异常

            return m_values;
        }

        /**
        * @将xml转为WxPayData对象并返回对象内部的数据
        * @param string 待转换的xml串
        * @return 经转换得到的Dictionary
        * @throws WxPayException
        */
        public SortedDictionary<string, object> LoadXml(string xml, string trade_type, string keyvalue)
        {
            m_values = fromXML(xml);
            CheckSign(trade_type, keyvalue);//验证签名,不通过会抛异常
            return m_values;
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
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
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
            return JsonConvert.SerializeObject(m_values);
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

                    throw new Exception("WxPayData内部含有值为null的字段!");
                }

                str += string.Format("{0}={1}<br>", pair.Key, pair.Value.ToString());
            }
            return str;
        }

        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名
        */
        public string MakeSign(string keyvalue)
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY

            //if (keyvalue == "APP")
            //{
            //    keyvalue = BasicConfig.App_AppKey;
            //}
            //else if (keyvalue == "JSAPI" || string.IsNullOrEmpty(keyvalue))
            //{
            //    keyvalue = BasicConfig.WeChat_AppKey;
            //}
            //else if (keyvalue == "NATIVE")
            //{
            //    keyvalue = BasicConfig.WeChat_AppKey;
            //}
            //else if (keyvalue == "MWEB")
            //{
            //    keyvalue = BasicConfig.H5_AppKey;
            //}

            str += "&key=" + keyvalue;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }

            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        public string MakeSign(string trade_type, string keyvalue)
        {
            string key = keyvalue;
            //if (trade_type == "APP")
            //{
            //    key = WebConfig.App_AppKey;
            //}
            //else if (trade_type == "JSAPI" || string.IsNullOrEmpty(trade_type))
            //{
            //    key = WebConfig.Wechat_AppKey;
            //}
            //else if (trade_type == "NATIVE")
            //{
            //    key = WebConfig.Native_AppKey;
            //}
            //else if (trade_type == "MWEB")
            //{
            //    key = WebConfig.H5_AppKey;
            //}
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            // str += "&key=" + WxPayConfig.KEY;
            str += "&key=" + key;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign(string keyvalue)
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
                return true;
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                //Log.error("WxPayData签名存在但不合法!");
                throw new Exception("WxPayData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = MakeSign(keyvalue);

            if (cal_sign == return_sign)
            {
                return true;
            }

            throw new Exception("WxPayData签名验证错误!");
        }

        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign(string trade_type, string keyvalue)
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
                return true;
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                throw new Exception("WxPayData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();
            //在本地计算新的签名
            string cal_sign = MakeSign(trade_type, keyvalue);

            if (cal_sign == return_sign)
            {
                return true;
            }

            throw new Exception("WxPayData签名验证错误!");
        }

        /**
        * @获取Dictionary
        */
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }

        private static SortedDictionary<string, object> fromXML(string xml)
        {
            SortedDictionary<string, object> result = null;

            try
            {
                if (string.IsNullOrEmpty(xml))
                {
                    return null;
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.XmlResolver = null;
                xmlDoc.LoadXml(xml);
                XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
                XmlNodeList nodes = xmlNode.ChildNodes;

                result = new SortedDictionary<string, object>();
                foreach (XmlNode xn in nodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    result[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public bool SetValues(SortedDictionary<string, object> values)
        {
            m_values = values;

            string trade_type = "";
            string keyvalue = "";

            if (m_values.ContainsKey("trade_type"))
            {
                trade_type = m_values["trade_type"].ToString();
            }

            if (trade_type == "APP")
            {
                keyvalue = BasicConfig.App_AppKey;
            }
            else if (trade_type == "JSAPI" || string.IsNullOrEmpty(trade_type))
            {
                keyvalue = BasicConfig.WeChat_AppKey;
            }
            else if (trade_type == "NATIVE")
            {
                keyvalue = BasicConfig.WeChat_AppKey;
            }
            else if (trade_type == "MWEB")
            {
                keyvalue = BasicConfig.H5_AppKey;
            }

            return CheckSign(keyvalue);//验证签名,不通过会抛异常
        }

    }
}