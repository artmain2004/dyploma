using Microsoft.AspNetCore.Identity;

namespace Identity.Extensions
{
    public static class RolesExtension
    {
        public static async Task ApplyRoles(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var listRoles = new List<string> { "Admin", "User" };

            foreach(var role in listRoles)
            {
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
            }
        }


    }
}
