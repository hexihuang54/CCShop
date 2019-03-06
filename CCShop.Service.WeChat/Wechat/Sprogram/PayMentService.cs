using CCShop.Service.WeChat.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCShop.Service.WeChat.Sprogram
{
    public class PayMentService
    {
        public ResultModel<WeChatPayResultModel> SProgramPay(PayParamModel model)
        {
            var return_Url = WrapConfig.WeChatCallBackUrl;

            //统一下单
            WeChatPayData data = new WeChatPayData();

            if (string.IsNullOrEmpty(model.title))
            {
                model.title = "订单充值";
            }

            var orderamount = ((int)(model.price * 100)).ToString();

            data.SetValue("body", model.title);
            data.SetValue("detail", model.title);
            data.SetValue("attach", "TodayGoods");
            data.SetValue("out_trade_no", model.out_trade_osn);
            data.SetValue("total_fee", orderamount);
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", model.openid);
            data.SetValue("spbill_create_ip", model.spbill_create_ip);
            data.SetValue("notify_url", return_Url);

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
                            code = 1,
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
                            code = 1,
                            msg = "该订单已支付",
                            data = payResult
                        };
                    }
                    else if (result.GetValue("err_code_des").ToString() == "余额不足")
                    {
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 1,
                            msg = "余额不足",
                            data = payResult
                        };
                    }
                    else if (result.GetValue("err_code_des").ToString() == "商户订单号重复")
                    {
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 1,
                            msg = "商户订单号重复",
                            data = payResult
                        };
                    }
                    else if (result.GetValue("err_code_des").ToString() == "订单已关闭")
                    {
                        return new ResultModel<WeChatPayResultModel>()
                        {
                            code = 1,
                            msg = "订单已关闭",
                            data = payResult
                        };
                    }
                }

                return new ResultModel<WeChatPayResultModel>()
                {
                    code = 1,
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


        private string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private string GetJsApiParameters(string keyvalue, string appid, string prepayid)
        {
            WeChatPayData jsApiParam = new WeChatPayData();

            jsApiParam.SetValue("appId", appid);
            jsApiParam.SetValue("timeStamp", GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + prepayid);
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign(keyvalue));

            string parameters = jsApiParam.ToJson();

            return parameters;
        }

        private WeChatPayData UnifiedOrder(WeChatPayData inputObj, int timeOut = 6)
        {
            try
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

                //NATIVE关联参数
                if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
                {
                    throw new Exception("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
                }

                //回调url
                if (!inputObj.IsSet("notify_url"))
                {
                    throw new Exception("统一支付接口中，缺少必填参数notify_url！trade_type为JSAPI时，notify_url为必填参数！");
                }

                //支付场景
                string trade_type = inputObj.GetValue("trade_type").ToString();
                string keyvalue = "";


                if (trade_type == "JSAPI")
                {
                    inputObj.SetValue("appid", BasicConfig.Sprogram_AppId);//公众账号ID
                    inputObj.SetValue("mch_id", BasicConfig.WeChat_MchId);//商户号

                    if (trade_type == "JSAPI")
                    {
                        keyvalue = BasicConfig.WeChat_AppKey;
                    }
                }

                inputObj.SetValue("nonce_str", GenerateNonceStr());                 //随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign(trade_type, keyvalue)); //签名

                string xml = inputObj.ToXml();

                //Log.Info("xml:" + xml);

                string response = WHttpUtil.Post(xml, url, false, timeOut);

                WeChatPayData result = new WeChatPayData();

                result.LoadXml(response, trade_type, keyvalue);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
    }
}
