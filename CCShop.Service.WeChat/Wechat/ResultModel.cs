namespace CCShop.Service.WeChat

{
    public class ResultModel<T>
    {
        public int code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }

    public class ErroResult
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}