using CCShop.Log4Net;
using CCShop.Service.WeChat.Common;
using CCShop.Service.WeChat.Wrap.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CCShop.Service.WeChat.Wrap
{
    public class UserService
    {
        public async Task<ResultModel<UserInfoModel>> GetUserInfoDataAsync(string accesstoken, string openid)
        {
            var url = string.Format(WrapConfig.UserInfoUrl, accesstoken, openid);
            var jsonResult = await WHttpUtil.HttpGetAsync(url);
            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                var result = JsonConvert.DeserializeObject<UserInfoModel>(jsonResult);
                if (result == null || !string.IsNullOrWhiteSpace(result.errcode))
                {
                    return new ResultModel<UserInfoModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<UserInfoModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }
            return null;
        }

        public async Task<ResultModel<UserInfoModel>> GetUser_InfoDataAsync(string accesstoken, string openid)
        {
            var url = string.Format(WrapConfig.User_InfoUrl, accesstoken, openid);
            var jsonResult = await WHttpUtil.HttpGetAsync(url);
            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                var result = JsonConvert.DeserializeObject<UserInfoModel>(jsonResult);
                if (result == null || !string.IsNullOrWhiteSpace(result.errcode))
                {
                    return new ResultModel<UserInfoModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<UserInfoModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }
            return null;
        }


        public ResultModel<UserInfoModel> GetUserInfoData(string accesstoken, string openid)
        {
            var url = string.Format(WrapConfig.UserInfoUrl, accesstoken, openid);
            var jsonResult = WHttpUtil.HttpGet(url);
            if (!string.IsNullOrWhiteSpace(jsonResult))
            {

                var result = JsonConvert.DeserializeObject<UserInfoModel>(jsonResult);

                if (result == null || !string.IsNullOrWhiteSpace(result.errcode))
                {
                    return new ResultModel<UserInfoModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<UserInfoModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }
            return null;
        }

        public ResultModel<UserInfoModel> GetUser_InfoData(string accesstoken, string openid)
        {
            var url = string.Format(WrapConfig.User_InfoUrl, accesstoken, openid);
            var jsonResult = WHttpUtil.HttpGet(url);
            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                var result = JsonConvert.DeserializeObject<UserInfoModel>(jsonResult);
                if (result == null || !string.IsNullOrWhiteSpace(result.errcode))
                {
                    return new ResultModel<UserInfoModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }
                return new ResultModel<UserInfoModel>
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