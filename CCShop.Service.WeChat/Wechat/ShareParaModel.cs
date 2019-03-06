using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CCShop.Service.WeChat
{
    public class ShareParaModel
    {
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
    }
}
