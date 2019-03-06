using System;
using System.Collections.Generic;
using System.Text;

namespace CCShop.Service.WeChat.Sprogram.Model
{
    public class LoginSessionModel
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
    }
}
