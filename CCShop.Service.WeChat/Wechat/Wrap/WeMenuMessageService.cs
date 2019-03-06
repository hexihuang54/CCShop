using CCShop.Log4Net;
using CCShop.Service.WeChat.Common;
using CCShop.Service.WeChat.Wechat.Wrap.Model;
using CCShop.Service.WeChat.Wrap;
using Newtonsoft.Json;
using System;

namespace CCShop.Service.WeChat.Wechat.Wrap
{
    public class WeMenuMessageService
    {
        public ResultModel<WechatStatusModel> CreateMenu(string appid, string appsecret, string menuhtml)
        {
            try
            {
                AuthorService author = new AuthorService();
                var token = author.GetBaseAccessTokenData();
                if (token != null && token.code == 1 && !string.IsNullOrWhiteSpace(token.data.access_token))
                {
                    string createurl = string.Format(WrapConfig.MenuCreate, token.data.access_token);
                    var jsonresult = WHttpUtil.Get(createurl, menuhtml);
                    var err = JsonConvert.DeserializeObject<ErroResult>(jsonresult);
                    if (err.errmsg == "ok")
                    {
                        return new ResultModel<WechatStatusModel>
                        {
                            code = 1,
                            msg = "绑定微信目录成功",
                            data = new WechatStatusModel()
                            {
                                msg = err.errmsg,
                                status = true
                            }
                        };
                    }
                    else
                    {
                        return new ResultModel<WechatStatusModel>
                        {
                            code = 0,
                            msg = "绑定微信目录失败",
                            data = new WechatStatusModel()
                            {
                                msg = "错误代码:" + err.errcode + "," + err.errmsg,
                                status = false
                            }
                        };
                    }
                }
                else
                {
                    return new ResultModel<WechatStatusModel>
                    {
                        code = 0,
                        msg = "token获取为空",
                        data = null
                    };
                }
            }
            catch (Exception e)
            {
                Log.Err(e, "CreateMenu创建菜单异常");
                return new ResultModel<WechatStatusModel>
                {
                    code = 0,
                    msg = e.Message,
                    data = null
                };
            }
        }


        /// <summary>
        /// 上传图片、语音、视频、缩略图片永久素材
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <param name="type">image、voice、video、thumb</param>
        /// <param name="media"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public ResultModel<ForeverMaterialModel> AppendForeverMaterial(string appid, string appsecret, string type, byte[] bt, string filename)
        {
            try
            {
                AuthorService author = new AuthorService();
                var token = author.GetBaseAccessTokenData();
                if (token != null && token.code == 1 && token.data != null && !string.IsNullOrWhiteSpace(token.data.access_token))
                {
                    string url = string.Format(WrapConfig.AddMaterial + token.data.access_token + "&type=" + type);
                    var result = WHttpUtil.HttpMediaIdPost(url, bt, filename);
                    if (result.Contains("errcode"))
                    {
                        //异常
                        var errorinfo = JsonConvert.DeserializeObject<ErroResult>(result);
                        return new ResultModel<ForeverMaterialModel>
                        {
                            code = 0,
                            msg = errorinfo.errmsg,
                            data = null
                        };
                    }
                    else
                    {
                        var material = JsonConvert.DeserializeObject<ForeverMaterialModel>(result);
                        return new ResultModel<ForeverMaterialModel>
                        {
                            code = 1,
                            msg = "上传素材成功",
                            data = material
                        };
                    }
                }

                return new ResultModel<ForeverMaterialModel>
                {
                    code = 0,
                    msg = "token获取为空",
                    data = null
                };
            }
            catch (Exception e)
            {
                Log.Err(e, "上传永久素材异常");
                return new ResultModel<ForeverMaterialModel>
                {
                    code = 0,
                    msg = e.Message,
                    data = null
                };
            }
        }
    }
}
