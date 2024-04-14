using EasyTalkWeb.Hubs;
using EasyTalkWeb.Identity;
using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          
            builder.Services.AddPersistanceServices(builder.Configuration);
            builder.Services.AddIdentityServices();
            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<Person>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IMailService, MailService>();

            var app = builder.Build();
            RoleSeeder.SeedRolesAsync(app).Wait();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.MapIdentityApi<Person>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
