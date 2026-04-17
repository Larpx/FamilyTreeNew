using System.IO.Compression;
using FamilyTreeNew.BLL.Extensions;
using FamilyTreeNew.DAL.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using CacheControlHeaderValue = Microsoft.Net.Http.Headers.CacheControlHeaderValue;
using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

namespace FamilyTreeNew.Web;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        builder.Services.AddHttpClient();

        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
            options.Cookie.Name = "XSRF-TOKEN";
            options.Cookie.HttpOnly = false;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

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

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Admin/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
            });

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                               ?? builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDalServices(connectionString!);
        builder.Services.AddBllServices(builder.Configuration);

        var dataProtectionBuilder = builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
            .SetApplicationName("FamilyTreeNew");

        if (OperatingSystem.IsWindows())
        {
            dataProtectionBuilder.ProtectKeysWithDpapi();
        }

        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        var app = builder.Build();

        app.UseExceptionHandler("/Home/Error");

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Home/StatusCodePage", "?code={0}");

        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

            var cspPolicy = "default-src 'self'; " +
                            "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.font.im https://cdn.jsdelivr.net; " +
                            "font-src 'self' https://fonts.gstatic.com https://fonts.font.im https://cdn.jsdelivr.net; " +
                            "img-src 'self' data: blob:; " +
                            "connect-src 'self'; " +
                            "frame-ancestors 'self'; " +
                            "base-uri 'self'; " +
                            "form-action 'self'";

            context.Response.Headers[HeaderNames.ContentSecurityPolicy] = cspPolicy;

            await next();
        });

        app.UseHttpsRedirection();
        if (!app.Environment.IsDevelopment())
        {
            app.UseResponseCompression();
        }
        app.UseResponseCaching();

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(365)
                };
            }
        });

        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
