using System.Security.Claims;
using System.Security.Principal;

namespace CCShop.Common.Http
{

    public static class IdentityExtension
    {
        public static string GetValue(this IIdentity identity, string key)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(key);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
