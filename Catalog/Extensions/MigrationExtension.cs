using Microsoft.EntityFrameworkCore;

namespace CatalogService.Extensions;

public static class MigrationExtension
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Data.CatalogDbContext>();
        dbContext.Database.Migrate();
    }
}
