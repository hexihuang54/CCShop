using CCShop.Service.WeChat.Common;

namespace CCShop.Service.WeChat
{

    public class WrapConfig
    {
        public static string AccessTokenUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["AccessTokenUrl"];
            }
        }

        public static string BaseAccessTokenUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["BaseAccessTokenUrl"];
            }
        }

        public static string PcAccessTokenUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["PcAccessTokenUrl"];
            }
        }

        public static string SnsApiBaseUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["SnsApiBaseUrl"];
            }
        }

        public static string SnsApiUserInfoUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["SnsApiUserInfoUrl"];
            }
        }

        public static string QrConnectUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["QrConnectUrl"];
            }
        }

        public static string UserInfoUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["UserInfoUrl"];
            }
        }

        public static string User_InfoUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["User_InfoUrl"];
            }
        }

        public static string UnifiedorderUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["UnifiedorderUrl"];
            }
        }

        public static string WeChatCallBackUrl
        {
            get
            {
                return WechatConfigurationManager.AppSettings["WeChatCallBackUrl"];
            }
        }


        public static string WeChatPcRedirectUri
        {
            get
            {
                return WechatConfigurationManager.AppSettings["WeChatPcRedirectUri"];
            }
        }

        public static string WeChatRedirectUri
        {
            get
            {
                return WechatConfigurationManager.AppSettings["WeChatRedirectUri"];
            }
        }

        public static string MenuCreate
        {
            get
            {
                return WechatConfigurationManager.AppSettings["MenuCreate"];
            }
        }

        public static string AddMaterial
        {
            get
            {
                return WechatConfigurationManager.AppSettings["AddMaterial"];
            }
        }
    }


    public class BasicConfig
    {
        public static string App_AppId
        {
            get
            {
                return WechatConfigurationManager.AppSettings["App_AppId"];
            }
        }
        public static string App_MchId
        {
            get
            {
                return WechatConfigurationManager.AppSettings["App_MchId"];
            }
        }


        public static string WeChat_Secret
        {
            get
            {
                return WechatConfigurationManager.AppSettings["WeChat_Secret"];
            }
        }

        public static string WeChat_AppId
        {
            get
            {
                return WechatConfigurationManager.AppSettings["WeChat_AppId"];
            }
        }
        public static string Wechat_PayaAppId
        {
            get
            {
                return WechatConfigurationManager.AppSettings["Wechat_PayaAppId"];
            }
        }
        public static string WeChat_MchId
        {
            get
            {
                return WechatConfigurationManager.AppSettings["WeChat_MchId"];
            }
        }


        public static string Pc_AppId
        {
            get
            {
                return WechatConfigurationManager.AppSettings["Pc_AppId"];
            }
        }
        public static string PcAppSecret
        {
            get
            {
                return WechatConfigurationManager.AppSettings["PcAppSecret"];
            }
        }

        public static string Sprogram_AppId
        {
            get
            {
                return WechatConfigurationManager.AppSettings["Sprogram_AppId"];
            }
        }
        public static string SprogramAppSecret
        {
            get
            {
                return WechatConfigurationManager.AppSettings["SprogramAppSecret"];
            }
        }


        public static string App_AppKey
        {
            get
            {
                return WechatConfigurationManager.AppSettings["App_AppKey"];
            }
        }
        public static string WeChat_AppKey
        {
            get
            {
                return WechatConfigurationManager.AppSettings["WeChat_AppKey"];
            }
        }
        public static string H5_AppKey
        {
            get
            {
                return WechatConfigurationManager.AppSettings["H5_AppKey"];
            }
        }
    }


    public class SprogramConfig
    {
        public static string jscode2session
        {
            get
            {
                return WechatConfigurationManager.AppSettings["jscode2session"];
            }
        }
    }


}