namespace CCShop.Service.WeChat.Wrap.Model
{
    public class TicketModel
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public int expires_in { get; set; }
    }
}