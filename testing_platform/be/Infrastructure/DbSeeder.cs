using Domain.dbo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DbSeeder
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        await SeedRoles(serviceProvider);
        await SeedAdministrator(serviceProvider);
    }

    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        string[] roleNames = ["ADMINISTRATOR", "STUDENT", "AUTHOR"];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }
    }

    public static async Task SeedAdministrator(IServiceProvider serviceProvider)
    {
        var user = new User
        {
            UserName = "InternalAdmin",
            Email = "internalAdmin@gmail.com"
        };

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(user, "Qwerty123$");
    }
}