namespace CCShop.Service.WeChat.Wrap.Model
{
    public class UserInfoModel
    {
        public string openid { get; set; } 
        public string nickname { get; set; }
        public int sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public object privilege { get; set; }
        public string unionid { get; set; }
        public int subscribe { get; set; }
        public long subscribe_time { get; set; }

        public string errcode { get; set; }
        public string errmsg { get; set; }

    }
}