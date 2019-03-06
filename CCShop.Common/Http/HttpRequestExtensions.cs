using Microsoft.AspNetCore.Http;
using System.Text;

namespace CCShop.Common.Http
{
    public static class HttpRequestExtensions
    {
        public static string GetAbsoluteUri(this HttpRequest request) => new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
    }
}
