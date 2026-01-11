using Testcontainers.PostgreSql;
using FluentAssertions;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Identity.Tests;

public class IdentityIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("identity_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithImage("postgres:16")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    [Fact]
    public async Task CanCreateUserWithUserManager()
    {
        var services = new ServiceCollection();
        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext>();

        var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await db.Database.MigrateAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = new User { UserName = "test@local", Email = "test@local", Firstname = "Test", Lastname = "User", Age = 20 };
        var result = await userManager.CreateAsync(user, "Password1!");

        result.Succeeded.Should().BeTrue();
        var stored = await userManager.FindByEmailAsync("test@local");
        stored.Should().NotBeNull();
    }
}
