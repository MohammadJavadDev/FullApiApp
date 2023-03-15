using Data;
using Data.Contracts;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
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

            var app = builder.Build();

             if (!app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Error");
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
    }
}