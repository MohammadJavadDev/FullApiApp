using Data;
using Data.Contracts;
using Data.Repositories;
using Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.Text;

namespace WebFramework.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
       
            public static void AddCommonServices(this IServiceCollection services, ConfigurationManager configurationManager)
            {
                services.AddRazorPages();
 
                 

                services.AddMvc();

                services.AddHttpContextAccessor();

            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(configurationManager.GetConnectionString("Db"));
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

           


                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //    .AddJwtBearer(options =>
                //    {
                //        options.TokenValidationParameters = new TokenValidationParameters
                //        {
                //            ValidateIssuer = false,
                //            ValidateAudience = false,
                //            ValidateLifetime = true,
                //            ValidateIssuerSigningKey = true,
                //            ClockSkew = TimeSpan.Zero,
                //            RequireSignedTokens = true,
                //            RequireExpirationTime = true,
                //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SecretKey))
                //            ,
                //            TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SecretKey))
                //        };
                //    });



                //services.AddIdentity<User, Role>(op =>
                //{
                //    op.Password.RequireNonAlphanumeric = false;
                //    op.Password.RequireUppercase = false;
                //    op.Password.RequireLowercase = false;

                //    op.Lockout.MaxFailedAccessAttempts = 5;


                //}).AddEntityFrameworkStores<ApplicationDbContext>();


            }
      
    }
}
