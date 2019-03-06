using Microsoft.AspNetCore.Http;
using System;

namespace CCShop.Common.Http
{
    public static class HttpContext
    {
        public static IServiceProvider ServiceProvider;
        static HttpContext()
        { }
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
                Microsoft.AspNetCore.Http.HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

        public static CookieOptions CookieOptions
        {
            get
            {
                return new CookieOptions();
            }
        }
    }
}
