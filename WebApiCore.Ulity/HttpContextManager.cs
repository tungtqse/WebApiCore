using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiCore.Utility
{
    public class HttpContextManager
    {  
        public static Guid GetUserId()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)AppContext.Current.User.Identity;
            var Id = identity.Claims.FirstOrDefault(f => f.Type == "id");

            return Guid.TryParse(Id.Value, out var id) ? id : Guid.Empty;
        }

        public static string GetUserName()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)AppContext.Current.User.Identity;
            var name = identity.Claims.FirstOrDefault(f => f.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            return name;
        }

        public static string GetUserEmail()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)AppContext.Current.User.Identity;
            var email = identity.Claims.FirstOrDefault(f => f.Type == System.Security.Claims.ClaimTypes.Email).Value;

            return email;
        }
    }
}
