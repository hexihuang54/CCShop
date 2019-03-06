using System;
using System.Collections.Generic;
using System.Text;
using CCShop.Common.Util;

namespace CCShop.Common.Service
{
    public class ExpressDataApi
    {

        public static string KuaiDi100
        {
            get
            {
                return "http://www.kuaidi100.com/query?type={0}&postid={1}";
            }
        }

        public static ResultMode<ExpressResultModel> GetExpressDataByTypeCode(string type, string code)
        {
            var url = string.Format(KuaiDi100, type, code);

            var json = HttpUtil.Get(url, "");

            try
            {
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var result = JsonUtil.DeserializeObject<ExpressResultModel>(json);
                    if (result != null)
                    {
                        return new ResultMode<ExpressResultModel>
                        {
                            code = 0,
                            msg = "获取数据成功",
                            data = result
                        };
                    }
                }
                else
                {
                    return new ResultMode<ExpressResultModel>
                    {
                        code = 1,
                        msg = "获取数据失败,返回数据为空"
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }

    public class ExpressResultModel
    {
        public string message { get; set; }

        public string nu { get; set; }

        public int ischeck { get; set; }

        public string condition { get; set; }

        public string com { get; set; }

        public int status { get; set; }

        public int state { get; set; }

        public List<ExpressInfo> data { get; set; }
    }

    public class ExpressInfo
    {
        public string time { get; set; }

        public string ftime { get; set; }

        public string context { get; set; }

        public string location { get; set; }

    }

    public class ResultMode<T>
    {
        public int code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }

}
