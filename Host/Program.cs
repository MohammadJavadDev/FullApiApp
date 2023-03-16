using Data;
using Data.Contracts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Fluent;
using NLog.Web;
using WebFramework.Middlewares;

namespace Host
{
    
    public class Program
    {
        
        public static void Main(string[] args)
        {
 
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddMvcCore();
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("Db"));
            });
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.WebHost.ConfigureLogging(op => op.ClearProviders());
            builder.WebHost.UseNLog();

             
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Error("init main");
                var  app = builder.Build();
                if (!app.Environment.IsDevelopment())
                {
 
                    app.UseHsts();
                }
 

                app.UseCustomExceptionHandler();

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

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