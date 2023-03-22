using Common;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.SdkServices
{
    public class Sdk : ISdk, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUser CurrentUser { get; private set; }
        public Sdk(IHttpContextAccessor httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor;
            var userIdentity = _httpContextAccessor.HttpContext.User.Identity;
            CurrentUser = new CurrentUser
            {
                Id = userIdentity.GetUserId<string>(),
                Username = userIdentity.GetUserName(),
                FullName = userIdentity.FindFirstValue(ClaimTypes.GivenName),
                
            };
        }

        

    }
    public class CurrentUser
    {
        public string Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public Role[] roles { get; set; } = new Role[0];
    }
}
