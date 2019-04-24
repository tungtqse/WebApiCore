using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiCore.Utility
{
    public class ApplicationUser : IApplicationUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }        


        public Guid GetUserId()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var Id = identity.Claims.FirstOrDefault(f=>f.Type == "id");           

            return Guid.TryParse(Id.Value, out var id) ? id : Guid.Empty;
        }

        public string GetUserName()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var name = identity.Claims.FirstOrDefault(f=>f.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            return name;
        }

        public string GetUserEmail()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var email = identity.Claims.FirstOrDefault(f => f.Type == System.Security.Claims.ClaimTypes.Email).Value;

            return email;
        }
    }
}
