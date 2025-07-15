using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Utils
{
    public static class CookieUtil
    {
        public static void SetCookie(HttpResponse res, string name, string value)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7)
            };
            res.Cookies.Append(name, value, cookieOptions);
        }
    }
}