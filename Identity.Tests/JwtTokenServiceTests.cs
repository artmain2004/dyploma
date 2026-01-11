using FluentAssertions;
using Identity.Models;
using Identity.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Identity.Tests;

public class JwtTokenServiceTests
{
    [Fact]
    public void CreateAccessToken_IncludesRolesAndPermissions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "issuer",
                ["Jwt:Audience"] = "audience",
                ["Jwt:Key"] = "SUPER_SECRET_KEY_32_CHARS_MINIMUM_123",
                ["Jwt:AccessTokenMinutes"] = "10"
            })
            .Build();

        var service = new JwtTokenService(config);
        var user = new User { Id = Guid.NewGuid().ToString(), Email = "user@test.com", UserName = "user@test.com" };

        var token = service.CreateAccessToken(user, new[] { "Admin" }, new[] { "order:read" });
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        jwt.Claims.Should().Contain(c => c.Type == "permission" && c.Value == "order:read");
    }
}
