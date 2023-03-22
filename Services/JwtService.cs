using Common;
using Common.Utilities;
using Entities.Users;
using Microsoft.AspNetCore.Identity;
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
    public class JwtService : IJwtService , IScopedDependency
    {
        private readonly SignInManager<User> signInManager;

        public JwtService(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }

        public async Task<string> GenerateAsync(User user)
        {
            var scKey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);
            var ecKeyTemp = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);

            // Note that the ecKey should have 256 / 8 length:
            byte[] ecKey = new byte[256 / 8];
            Array.Copy(ecKeyTemp, ecKey, 256 / 8);

            var secretKey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);

            var singingCredenatials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.RsaSha256
                );

            var encriptionkey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);
            var encriyptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encriptionkey),
                SecurityAlgorithms.Aes256CbcHmacSha512
                );

            var claims = await _getClamisAsync(user);

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        scKey),
                        SecurityAlgorithms.HmacSha512),
                            EncryptingCredentials = new EncryptingCredentials(
                    new SymmetricSecurityKey(
                        ecKey),
                        SecurityAlgorithms.Aes256KW,
                        SecurityAlgorithms.Aes256CbcHmacSha512),
                Subject = new ClaimsIdentity(claims)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
               
            var securityToken = handler.CreateToken(descriptor);

            var tokenJwt = handler.WriteToken(securityToken);

            return tokenJwt;
        }

        private async Task<IEnumerable<Claim>> _getClamisAsync(User user)
        {
            var res = await signInManager.ClaimsFactory.CreateAsync(user);
            
            return res.Claims;
            //var list = new List<Claim>()
            //{
            //    new Claim( ClaimTypes.GivenName ,user.FullName ?? "") ,
            //    new Claim( ClaimTypes.NameIdentifier,user.Id.ToString()) ,
            //    new Claim( ClaimTypes.Name,user.UserName ?? "") ,
            //};

            //var roles = new Role[] { new Role() { Name = "Admin" } };

            //foreach(var r in roles)
            //    list.Add(
            //        new Claim(ClaimTypes.Role,r.Name )
            //        );


            //return list;

        }
    }
}
