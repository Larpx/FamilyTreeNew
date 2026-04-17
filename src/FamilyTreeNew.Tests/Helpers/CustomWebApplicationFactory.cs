using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace FamilyTreeNew.Tests.Helpers;

/// <summary>
/// 自定义的 ASP.NET Core 测试宿主工厂。
/// 用于在集成测试中覆盖应用配置，并注入测试环境所需的 JWT 和数据库连接字符串。
/// </summary>
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
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Port=3306;Database=FamilyTreeDb_Test;User=root;Password=test_password;CharSet=utf8mb4;"
            });
        });

        builder.UseEnvironment("Development");
    }
}
