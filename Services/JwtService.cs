using Common.Utilities;
using Entities.Users;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JwtService : IJwtService
    {
        public string Generate(User user)
        {

            var secretKey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);

            var singingCredenatials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256
                );

            var encriptionkey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);
            var encriyptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encriptionkey),
                SecurityAlgorithms.HmacSha256);

            var claims = _getClamis(user);

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = singingCredenatials,
                EncryptingCredentials = encriyptingCredentials, 
                Subject = new ClaimsIdentity(claims)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(descriptor);

            var tokenJwt = handler.WriteToken(securityToken);

            return tokenJwt;
        }

        private IEnumerable<Claim> _getClamis(User user)
        {
            var list = new List<Claim>()
            {
                new Claim( ClaimTypes.GivenName ,user.FullName ?? "") ,
                new Claim( ClaimTypes.NameIdentifier,user.Id.ToString()) ,
                new Claim( ClaimTypes.Name,user.UserName ?? "") ,
            };

            var roles = new Role[] { new Role() { Name = "Admin" } };
           
            foreach(var r in roles)
                list.Add(
                    new Claim(ClaimTypes.Role,r.Name )
                    );
            

            return list;
        }
    }
}
