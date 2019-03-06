using CCShop.Service.WeChat.Common;
using CCShop.Service.WeChat.Sprogram.Model;
using Newtonsoft.Json;

namespace CCShop.Service.WeChat.Sprogram
{
    public class LoginService
    {

        public ResultModel<LoginSessionModel> Getjscode2session(string code)
        {
            var url = string.Format(SprogramConfig.jscode2session, BasicConfig.Sprogram_AppId, BasicConfig.SprogramAppSecret, code);

            var jsonResult = WHttpUtil.Get(url,"");

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                var result = JsonConvert.DeserializeObject<LoginSessionModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<LoginSessionModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<LoginSessionModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }


    }
}
