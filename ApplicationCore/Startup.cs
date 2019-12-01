using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ViewModels.Enums;

namespace ApplicationCore
{
    public class Startup
    {
        public static void Configure(IServiceCollection services)
        {
            
        }

        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var adminData = serviceProvider.GetService<IOptions<AdminDetailOptions>>().Value;
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleNames = Enum.GetValues(typeof(UserRoles)).Cast<UserRoles>().Select(x => x.ToString()).ToList();
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new Role(roleName));
                }
            }
            var user = await userManager.FindByEmailAsync(adminData.AdminEmail);
            if (user == null)
            {
                var admin = new User
                {

                    UserName = adminData.AdminEmail,
                    Email = adminData.AdminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    AdminData = new Admin()
                };

                var createPowerUser = await userManager.CreateAsync(admin, adminData.AdminPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, UserRoles.Admin.ToString());
                }
            }
        }
    }
}
