using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace System.Web.Mvc
{
    public static class Extend_User
    {
        public static int userID(this IIdentity identity)
        {

            if (identity.IsAuthenticated)
                return Convert.ToInt32((identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value);
            return -1;
        }

        public static string userName(this IIdentity identity)
        {
            if (identity.IsAuthenticated)
                return (identity as ClaimsIdentity).FindFirst("userName").Value;
            return "未知";
        }



        public static string userIP(this IPrincipal User)
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }
    }
}