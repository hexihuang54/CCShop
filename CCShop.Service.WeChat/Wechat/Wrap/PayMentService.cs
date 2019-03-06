using CCShop.Common.Util;
using CCShop.Log4Net;
using CCShop.Service.WeChat.Common;
using CCShop.Service.WeChat.Wrap.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CCShop.Service.WeChat.Wrap
{
    public class PayMentService
    {
        public ResultModel<WeChatPayData> WeChatPay(PayParamModel model)
        {


            //统一下单
            WeChatPayData data = new WeChatPayData();

            data.SetValue("body", model.title);
            data.SetValue("attach", model.title);
            //data.SetValue("detail", model.title);
            data.SetValue("out_trade_no", model.out_trade_osn);
            data.SetValue("total_fee", ((int)(model.price * 100)).ToString());
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", model.title);
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", model.openid);


            WeChatPayData result = UnifiedOrder(data);

            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                throw new Exception("UnifiedOrder response error!");
            }

            return new ResultModel<WeChatPayData>()
            {
                code = 1,
                msg = "微信支付平台接口调用成功",
                data = result
            };
        }

        public ResultModel<WeChatPayResultModel> H5Pay(PayParamModel model)
        {
            var return_Url = WrapConfig.WeChatCallBackUrl;

            //统一下单
            WeChatPayData data = new WeChatPayData();

            if (string.IsNullOrEmpty(model.title))
            {
                model.title = "订单充值";
            }

            var orderamount = ((int)(model.price * 100)).ToString();

            data.SetValue("body", model.title);//商品描述
            data.SetValue("attach", model.title);//附加数据
            data.SetValue("out_trade_no", model.out_trade_osn);//随机字符串
            data.SetValue("total_fee", orderamount);//总金额
            data.SetValue("trade_type", "MWEB");//交易类型
            data.SetValue("product_id", model.out_trade_osn);//商品ID
            data.SetValue("notify_url", return_Url); //回调URL
            data.SetValue("scene_info", model.scene_info);  //H5支付格式
            data.SetValue("spbill_create_ip", model.spbill_create_ip); //支付发起端IP地址

            WeChatPayData result = UnifiedOrder(data);

            if (result == null)
            {
                return new ResultModel<WeChatPayResultModel>()
                {
                    code = 0,
                    msg = "微信支付平台接口调用异常",
                    data = null
                };
            }

            WeChatPayResultModel payResult = new WeChatPayResultModel
            {
                prepay_id = result.GetValue("prepay_id") == null ? "" : result.GetValue("prepay_id").ToString(),
                sign = result.GetValue("sign") == null ? "" : result.GetValue("sign").ToString(),
                qrcodeurl = result.GetValue("code_url") == null ? "" : result.GetValue("code_url").ToString(),
                mch_id = result.GetValue("mch_id") == null ? "" : result.GetValue("mch_id").ToString(),
                mweb_url = result.GetValue("mweb_url") == null ? "" : result.GetValue("mweb_url").ToString()
            };

            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                if (result.GetValue("return_code") != null && result.GetValue("return_code").ToString() != "")
                {
                    if (result.GetValue("return_code").ToString() == "FAIL")
                    {
                        var return_msg = result.GetValue("return_msg").ToString();
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 0,
                            msg = return_msg,
                            data = payResult
                        };
                    }
                }

                if (result.GetValue("err_code_des") != null && result.GetValue("err_code_des").ToString() != "")
                {
                    if (result.GetValue("err_code_des").ToString() == "该订单已支付")
                    {
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 0,
                            msg = "该订单已支付",
                            data = payResult
                        };
                    }
                    else if (result.GetValue("err_code_des").ToString() == "余额不足")
                    {
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 0,
                            msg = "余额不足",
                            data = payResult
                        };
                    }
                    else if (result.GetValue("err_code_des").ToString() == "商户订单号重复")
                    {
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 0,
                            msg = "商户订单号重复",
                            data = payResult
                        };
                    }
                    else if (result.GetValue("err_code_des").ToString() == "订单已关闭")
                    {
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 0,
                            msg = "订单已关闭",
                            data = payResult
                        };
                    }
                }

                return new ResultModel<WeChatPayResultModel>()
                {
                    code = 0,
                    msg = "微信支付平台接口调用失败",
                    data = payResult
                };
            }


            var ApiResult = JsonConvert.DeserializeObject<ShareParaModel>(GetJsApiParameters(BasicConfig.WeChat_AppKey, result.GetValue("appid") == null ? "" : result.GetValue("appid").ToString(), result.GetValue("prepay_id") == null ? "" : result.GetValue("prepay_id").ToString()));
            payResult.timeStamp = ApiResult.timeStamp;
            payResult.nonceStr = ApiResult.nonceStr;
            payResult.package = ApiResult.package;
            payResult.signType = ApiResult.signType;
            payResult.paySign = ApiResult.paySign;
            payResult.appid = data.GetValue("appid").ToString();

            return new ResultModel<WeChatPayResultModel>()
            {
                code = 1,
                msg = "微信支付平台接口调用成功",
                data = payResult
            };
        }

        public string GetJsApiParameters(string keyvalue, string appid, string prepayid)
        {
            WeChatPayData jsApiParam = new WeChatPayData();

            jsApiParam.SetValue("appId", appid);
            jsApiParam.SetValue("timeStamp", GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + prepayid);
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign(keyvalue));

            string parameters = jsApiParam.ToJson();

            Log.Info(parameters);

            return parameters;
        }

        public string GetPrePayUrl(string out_trade_no)
        {
            Log.Info("微信端二维码生成开始...");

            WeChatPayData data = new WeChatPayData();
            data.SetValue("appid", BasicConfig.WeChat_AppId);//公众帐号id
            data.SetValue("mch_id", BasicConfig.WeChat_MchId);//商户号
            data.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            data.SetValue("product_id", out_trade_no);//商品ID
            data.SetValue("time_stamp", DateTimeUtil.DateTimeToUnixTimestamp(DateTime.Now));//时间戳
            data.SetValue("sign", data.MakeSign(BasicConfig.WeChat_AppKey));//签名
            string str = ToUrlParams(data.GetValues());//转换为URL串
            string url = "weixin://wxpay/bizpayurl?" + str;

            Log.Info("微信端二维码生成结束,二维码Url : " + url);

            return url;
        }

        private string ToUrlParams(SortedDictionary<string, object> map)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in map)
            {
                if (pair.Key != "sign")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff += "sign=" + map["sign"] + "&";

            buff = buff.Trim('&');
            return buff;
        }

        private WeChatPayData UnifiedOrder(WeChatPayData inputObj, int timeOut = 6)
        {
            string url = WrapConfig.UnifiedorderUrl;

            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new Exception("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new Exception("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new Exception("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new Exception("缺少统一支付接口必填参数trade_type！");
            }

            //JSAPI关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new Exception("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }

            //回调url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WrapConfig.WeChatCallBackUrl);
            }

            //支付场景

            inputObj.SetValue("appid", BasicConfig.Wechat_PayaAppId);//公众账号ID
            inputObj.SetValue("mch_id", BasicConfig.WeChat_MchId);//商户号
            inputObj.SetValue("spbill_create_ip", "8.8.8.8");//终端ip	  	
            inputObj.SetValue("nonce_str", GenerateNonceStr());                 //随机字符串
            string trade_type = inputObj.GetValue("trade_type").ToString();
            string keyvalue = BasicConfig.WeChat_AppKey;
            inputObj.SetValue("sign", inputObj.MakeSign(trade_type, keyvalue)); //签名

            string xml = inputObj.ToXml();

            Log.Info("支付请求参数xml:" + xml);

            string response = WHttpUtil.Post(xml, url, false, timeOut);

            WeChatPayData result = new WeChatPayData();

            result.LoadXml(response, trade_type, keyvalue);

            return result;


        }

        private string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
