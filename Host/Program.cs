using Data;
using Data.Contracts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Fluent;
using NLog.Web;
using Services;
using WebFramework.Middlewares;
using WebFramework.Bootstrap;
using Services.SdkServices;
using Microsoft.AspNetCore.Mvc.Authorization;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Autofac.Core;
using Autofac;

namespace Host
{
    
    public class Program
    {
        
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureLogging(op => op.ClearProviders());
            builder.WebHost.UseNLog();
            builder.Services.AddCommonServices(builder.Configuration);
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacBootsratp()));
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                //logger.Error("init main");
                var  app = builder.Build();
                if (!app.Environment.IsDevelopment())
                {
 
                    app.UseHsts();
                }
 

                app.UseCustomExceptionHandler();
            
                app.UseHttpsRedirection();
                app.UseStaticFiles();

            
              
                app.UseRouting();

                app.UseAuthentication();
                 app.UseAuthorization();


                app.MapControllers();

                app.MapRazorPages();
                app.Run();
            }
            catch (Exception ex)
            {
        
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
               LogManager.Shutdown();
            }
          

     
        }

    }
}