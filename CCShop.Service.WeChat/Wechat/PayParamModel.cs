namespace CCShop.Service.WeChat
{
    public class PayParamModel
    {
        public string out_trade_osn { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
        public string type { get; set; }
        public string openid { get; set; }
        public string spbill_create_ip { get; set; }
        public string scene_info { get; set; }
    }
}