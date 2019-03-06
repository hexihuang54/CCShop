using System.Runtime.Serialization;

namespace CCShop.Service.WeChat
{
    public class WeChatPayResultModel
    {
         [DataMember]
        public string appid { get; set; }

        // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
        [DataMember]
        public string timeStamp { get; set; }

        // 支付签名随机串，不长于 32 位
        [DataMember]
        public string nonceStr { get; set; }

        // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
        [DataMember]
        public string package { get; set; }

        // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
        [DataMember]
        public string signType { get; set; }

        // 支付签名
        [DataMember]
        public string paySign { get; set; }

        [DataMember]
        public string sign { get; set; }

        [DataMember]
        public string prepay_id { get; set; }

        [DataMember]
        public string qrcodeurl { get; set; }

        [DataMember]
        public string mch_id { get; set; }

        [DataMember]
        public string mweb_url { get; set; }
    }
}