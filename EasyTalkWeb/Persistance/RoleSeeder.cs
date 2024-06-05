using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace EasyTalkWeb.Persistance
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
                await SeedRoles(roleManager);
            }
        }

        private static async Task SeedRoles(RoleManager<UserRole> roleManager)
        {
            string[] roleNames = { "Admin", "Client", "Freelancer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new UserRole(roleName));
                }
            }
        }
    }
}
