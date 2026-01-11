using Microsoft.EntityFrameworkCore;

namespace Identity.Extensions
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Data.IdentityDbContext>();
            dbContext.Database.Migrate();
        }

    }
}
