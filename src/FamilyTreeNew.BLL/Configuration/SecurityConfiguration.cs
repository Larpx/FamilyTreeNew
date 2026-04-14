using Microsoft.Extensions.Configuration;

namespace FamilyTreeNew.BLL.Configuration;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "FamilyTreeNew";
    public string Audience { get; set; } = "FamilyTreeNewUsers";
    public int ExpirationMinutes { get; set; } = 120;
}

public class SecuritySettings
{
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 15;
    public int PasswordMinLength { get; set; } = 8;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecialChar { get; set; } = true;
    public int MaxRequestPerMinute { get; set; } = 60;
    public int MaxLoginAttemptsPerMinute { get; set; } = 5;
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
    public string[] AllowedHeaders { get; set; } = new[] { "Content-Type", "Authorization", "X-Requested-With" };
    public bool AllowCredentials { get; set; } = true;
    public int PreflightMaxAgeSeconds { get; set; } = 3600;
}

public static class ConfigurationExtensions
{
    public static JwtSettings GetJwtSettings(this IConfiguration configuration)
    {
        var settings = new JwtSettings();

        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                        ?? configuration["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be configured via JWT_SECRET_KEY environment variable or Jwt:SecretKey configuration");
        }

        if (secretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 characters long for security");
        }

        settings.SecretKey = secretKey;
        settings.Issuer = configuration["Jwt:Issuer"] ?? "FamilyTreeNew";
        settings.Audience = configuration["Jwt:Audience"] ?? "FamilyTreeNewUsers";
        settings.ExpirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var exp)
            ? exp
            : 120;

        return settings;
    }

    public static SecuritySettings GetSecuritySettings(this IConfiguration configuration)
    {
        var settings = new SecuritySettings();

        var section = configuration.GetSection("Security");
        if (section.Exists())
        {
            settings.MaxLoginAttempts = section.GetValue("MaxLoginAttempts", 5);
            settings.LockoutDurationMinutes = section.GetValue("LockoutDurationMinutes", 15);
            settings.PasswordMinLength = section.GetValue("PasswordMinLength", 8);
            settings.RequireUppercase = section.GetValue("RequireUppercase", true);
            settings.RequireLowercase = section.GetValue("RequireLowercase", true);
            settings.RequireDigit = section.GetValue("RequireDigit", true);
            settings.RequireSpecialChar = section.GetValue("RequireSpecialChar", true);
            settings.MaxRequestPerMinute = section.GetValue("MaxRequestPerMinute", 60);
            settings.MaxLoginAttemptsPerMinute = section.GetValue("MaxLoginAttemptsPerMinute", 5);
        }

        return settings;
    }

    public static CorsSettings GetCorsSettings(this IConfiguration configuration)
    {
        var settings = new CorsSettings();

        var section = configuration.GetSection("Cors");
        if (section.Exists())
        {
            var origins = section.GetSection("AllowedOrigins").Get<string[]>();
            if (origins != null && origins.Length > 0)
            {
                settings.AllowedOrigins = origins;
            }

            var methods = section.GetSection("AllowedMethods").Get<string[]>();
            if (methods != null && methods.Length > 0)
            {
                settings.AllowedMethods = methods;
            }

            var headers = section.GetSection("AllowedHeaders").Get<string[]>();
            if (headers != null && headers.Length > 0)
            {
                settings.AllowedHeaders = headers;
            }

            settings.AllowCredentials = section.GetValue("AllowCredentials", true);
            settings.PreflightMaxAgeSeconds = section.GetValue("PreflightMaxAgeSeconds", 3600);
        }

        return settings;
    }
}
