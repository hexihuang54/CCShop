using CCShop.Log4Net;
using CCShop.Service.WeChat.Common;
using CCShop.Service.WeChat.Wrap.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CCShop.Service.WeChat.Wrap
{
    public class AuthorService
    {
        #region 异步根据Code获取token

        public async Task<ResultModel<AccessTokenModel>> GetAccessTokenDataAync(string code)
        {
            var url = string.Format(WrapConfig.AccessTokenUrl, BasicConfig.WeChat_AppId, BasicConfig.WeChat_Secret, code);

            var jsonResult = await WHttpUtil.HttpGetAsync(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {

                var result = JsonConvert.DeserializeObject<AccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<AccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<AccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }

        public async Task<ResultModel<AccessTokenModel>> GetAccessTokenDataAync(string code, string appid, string secret)
        {
            var url = string.Format(WrapConfig.AccessTokenUrl, appid, secret, code);

            var jsonResult = await WHttpUtil.HttpGetAsync(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                var result = JsonConvert.DeserializeObject<AccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<AccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<AccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }


        public ResultModel<AccessTokenModel> GetAccessTokenData(string code)
        {
            var url = string.Format(WrapConfig.AccessTokenUrl, BasicConfig.WeChat_AppId, BasicConfig.WeChat_Secret, code);
            
            var jsonResult = WHttpUtil.Get(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {

                var result = JsonConvert.DeserializeObject<AccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<AccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<AccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }

        public ResultModel<AccessTokenModel> GetAccessTokenData(string code, string appid, string secret)
        {
            var url = string.Format(WrapConfig.AccessTokenUrl, appid, secret, code);
            
            var jsonResult = WHttpUtil.Get(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                var result = JsonConvert.DeserializeObject<AccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<AccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<AccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }

        #endregion


        #region 同步获取BaseToken

        public ResultModel<AccessTokenModel> GetBaseAccessTokenData()
        {
            var url = string.Format(WrapConfig.BaseAccessTokenUrl, BasicConfig.WeChat_AppId, BasicConfig.WeChat_Secret);

            var jsonResult = WHttpUtil.HttpGet(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {

                var result = JsonConvert.DeserializeObject<AccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<AccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<AccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }

        public ResultModel<AccessTokenModel> GetBaseAccessTokenData(string appid, string secret)
        {
            var url = string.Format(WrapConfig.BaseAccessTokenUrl, appid, secret);

            var jsonResult = WHttpUtil.HttpGet(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {

                var result = JsonConvert.DeserializeObject<AccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<AccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<AccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }

        #endregion


        #region 同步根据Code获取PCtoken

        public ResultModel<PcAccessTokenModel> GetPcAccessTokenData(string code)
        {

            var url = string.Format(WrapConfig.PcAccessTokenUrl, BasicConfig.Pc_AppId, BasicConfig.PcAppSecret, code);

            var jsonResult = WHttpUtil.HttpGet(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {

                var result = JsonConvert.DeserializeObject<PcAccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<PcAccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<PcAccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }

        public ResultModel<PcAccessTokenModel> GetPcAccessTokenData(string code, string appid, string secret)
        {
            var url = string.Format(WrapConfig.PcAccessTokenUrl, appid, secret, code);

            var jsonResult = WHttpUtil.HttpGet(url);

            if (!string.IsNullOrWhiteSpace(jsonResult))
            {

                var result = JsonConvert.DeserializeObject<PcAccessTokenModel>(jsonResult);

                if (result == null)
                {
                    return new ResultModel<PcAccessTokenModel>
                    {
                        code = 0,
                        msg = jsonResult,
                        data = null
                    };
                }

                return new ResultModel<PcAccessTokenModel>
                {
                    code = 1,
                    msg = jsonResult,
                    data = result
                };
            }

            return null;
        }

        #endregion


        #region 生成微信端获取Code的Url链接

        public static string GetWechatCodeUrl(string redirect_uri, bool isStatic = false)
        {
            if (!isStatic)
            {
                return string.Format(WrapConfig.SnsApiUserInfoUrl, BasicConfig.WeChat_AppId, redirect_uri);
            }
            else
            {
                return string.Format(WrapConfig.SnsApiBaseUrl, BasicConfig.WeChat_AppId, redirect_uri);
            }
        }

        public static string GetWechatCodeUrl(string redirect_uri, string appid, bool isStatic = false)
        {
            if (!isStatic)
            {
                return string.Format(WrapConfig.SnsApiUserInfoUrl, appid, redirect_uri);
            }
            else
            {
                return string.Format(WrapConfig.SnsApiBaseUrl, appid, redirect_uri);
            }
        }

        #endregion


        #region 生成PC端获取Code的Url链接

        public static string GetWeChatPcCodeUrl(string redirect_uri)
        {
            return string.Format(WrapConfig.QrConnectUrl, BasicConfig.Pc_AppId, redirect_uri);
        }

        public static string GetWeChatPcCodeUrl(string redirect_uri, string appid)
        {
            return string.Format(WrapConfig.QrConnectUrl, appid, redirect_uri);
        }

        #endregion
    }
}