using System.IO.Compression;
using System.Text;
using FamilyTreeNew.Api.Middleware;
using FamilyTreeNew.BLL.Configuration;
using FamilyTreeNew.BLL.Extensions;
using FamilyTreeNew.DAL.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;

namespace FamilyTreeNew.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
            options.Cookie.Name = "XSRF-TOKEN";
            options.Cookie.HttpOnly = false;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        builder.Services.AddResponseCaching();

        builder.Services.AddMemoryCache();

        var jwtSettings = builder.Configuration.GetJwtSettings();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }

                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("RequireUserRole", policy => policy.RequireAuthenticatedUser());
        });

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
            .SetApplicationName("FamilyTreeNew");

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                               ?? builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDalServices(connectionString!);
        builder.Services.AddBllServices(builder.Configuration);

        var corsSettings = builder.Configuration.GetCorsSettings();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("SecureCorsPolicy", policy =>
            {
                if (corsSettings.AllowedOrigins.Length > 0)
                {
                    policy.WithOrigins(corsSettings.AllowedOrigins);
                }
                else
                {
                    if (corsSettings.AllowCredentials)
                    {
                        policy.WithOrigins("https://localhost:5001", "https://localhost:5002");
                    }
                    else
                    {
                        policy.AllowAnyOrigin();
                    }
                }

                if (corsSettings.AllowCredentials)
                {
                    policy.AllowCredentials();
                }
                else
                {
                    policy.DisallowCredentials();
                }

                policy
                    .WithMethods(corsSettings.AllowedMethods)
                    .WithHeaders(corsSettings.AllowedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.PreflightMaxAgeSeconds));
            });
        });

        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseGlobalExceptionHandler();
        app.UseSecurityHeaders();
        app.UseXssProtection();
        app.UseRateLimiting();

        app.UseHttpsRedirection();

        app.UseResponseCompression();

        app.UseResponseCaching();

        app.UseCors("SecureCorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
