using System;
using System.Linq;
using FossTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FossTech.Data
{
    public static class SeedData
    {
        public static async void InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // await context.Database.EnsureDeletedAsync();

            // await context.Database.EnsureCreatedAsync();


            // await context.SaveChangesAsync();
          

            if (!context.Users.Any())
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = "kishor",
                    LastName = "Gujar",
                    Email = "superdashboard@fosstech.co",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "superdashboard@fosstech.co",
                    PhoneNumber = "+919820787902"
                };


                var adminUser = await userManager.CreateAsync(user, "Dashboard@0207");

                if (adminUser.Succeeded)
                {

                    await roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = "Admin"
                    });
                    await roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = "User"
                    });
                    await roleManager.CreateAsync(new IdentityRole()
                    {
                        Name="OutSideUser"
                    });

                    await userManager.AddToRoleAsync(user, "Admin");
                }

                await context.SaveChangesAsync();
            }

            if (!context.Pages.Any())
            {
                context.Add(new Page()
                {
                    Name = "return-policy",
                    Slug = "return-policy",
                    Content = "Return Policy Content",
                    Order = 1,
                });
                context.Add(new Page()
                {
                    Name = "privacy-policy",
                    Slug = "privacy-policy",
                    Content = "privacy policy Content",
                    Order = 1,
                });
                context.Add(new Page()
                {
                    Name = "terms-and-conditions",
                    Slug = "terms-and-conditions",
                    Content = "terms-and-conditions",
                    Order = 1,
                });

                await context.SaveChangesAsync();
            }

            if (await context.WebSettings.CountAsync() > 1)
            {
                var websetting = await context.WebSettings.Skip(1).ToListAsync();
                context.RemoveRange(websetting);
                await context.SaveChangesAsync();
            }
            if (!await context.WebSettings.AnyAsync())
            {
                context.Add(new WebSetting());
                await context.SaveChangesAsync();

            }
        }
    }
}
