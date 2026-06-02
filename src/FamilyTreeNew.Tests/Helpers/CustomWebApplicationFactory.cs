using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeNew.Tests.Helpers;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "ThisIsATestSecretKeyThatIsAtLeast32CharactersLong!");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "ThisIsATestSecretKeyThatIsAtLeast32CharactersLong!",
                ["Jwt:Issuer"] = "FamilyTreeNew",
                ["Jwt:Audience"] = "FamilyTreeNewUsers",
                ["Jwt:ExpirationMinutes"] = "120",
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Port=3306;Database=FamilyTreeDb_Test;User=root;Password=test_password;CharSet=utf8mb4;",
                ["Security:MaxLoginAttempts"] = "5",
                ["Security:LockoutDurationMinutes"] = "15",
                ["Security:PasswordMinLength"] = "8",
                ["Security:RequireUppercase"] = "true",
                ["Security:RequireLowercase"] = "true",
                ["Security:RequireDigit"] = "true",
                ["Security:RequireSpecialChar"] = "true",
                ["Security:MaxRequestPerMinute"] = "60",
                ["Security:MaxLoginAttemptsPerMinute"] = "5"
            });
        });

        builder.UseEnvironment("Development");
    }
}
