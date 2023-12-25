using PrespaEvents.Web.Models.Identity;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using PrespaEvents.Web.Constants;

namespace PrespaEvents.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetRequiredService<UserManager<EventApplicationUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Organizer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // creating admin

            var Admin_User = new EventApplicationUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserCart = new Models.Domain.Cart()
            };
            var admin_userInDb = await userManager.FindByEmailAsync(Admin_User.Email);
            if (admin_userInDb == null)
            {
                await userManager.CreateAsync(Admin_User, "Test123!");
                await userManager.AddToRoleAsync(Admin_User, Roles.Admin.ToString());
            }

            var OrganizerUser = new EventApplicationUser
            {
                UserName = "organizer@organizer.com",
                Email = "organizer@organizer.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserCart = new Models.Domain.Cart()
            };
            var organizer_userInDb = await userManager.FindByEmailAsync(OrganizerUser.Email);
            if (organizer_userInDb == null)
            {
                await userManager.CreateAsync(OrganizerUser, "Test123!");
                await userManager.AddToRoleAsync(OrganizerUser, Roles.Organizer.ToString());
            }

            var StandardUser = new EventApplicationUser
            {
                UserName = "user@user.com",
                Email = "user@user.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserCart = new Models.Domain.Cart()
            };
            var standard_userInDb = await userManager.FindByEmailAsync(StandardUser.Email);
            if (standard_userInDb == null)
            {
                await userManager.CreateAsync(StandardUser, "Test123!");
                await userManager.AddToRoleAsync(StandardUser, Roles.User.ToString());
            }
        }
    }
}
