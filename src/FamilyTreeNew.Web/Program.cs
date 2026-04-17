// 引入业务逻辑层的扩展方法
using FamilyTreeNew.BLL.Extensions;
// 引入数据访问层的扩展方法
using FamilyTreeNew.DAL.Extensions;
// 引入Cookie认证相关的命名空间
using Microsoft.AspNetCore.Authentication.Cookies;
// 引入数据保护相关的命名空间
using Microsoft.AspNetCore.DataProtection;
// 引入响应压缩相关的命名空间
using Microsoft.AspNetCore.ResponseCompression;
// 引入压缩相关的命名空间
using System.IO.Compression;
// 引入缓存控制头值
using CacheControlHeaderValue = Microsoft.Net.Http.Headers.CacheControlHeaderValue;
// 引入HTTP头名称
using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

// 定义命名空间
namespace FamilyTreeNew.Web;

// 定义Program类
public partial class Program
{
    // 程序的主入口方法
    public static void Main(string[] args)
    {
        // 创建Web应用程序构建器
        var builder = WebApplication.CreateBuilder(args);

        // 注册控制器和视图服务
        builder.Services.AddControllersWithViews();

        // 注册HttpClient服务
        builder.Services.AddHttpClient();

        // 配置反伪造令牌保护
        builder.Services.AddAntiforgery(options =>
        {
            // 设置反伪造令牌的头部名称
            options.HeaderName = "X-XSRF-TOKEN";
            // 设置反伪造令牌的Cookie名称
            options.Cookie.Name = "XSRF-TOKEN";
            // 设置Cookie不是HttpOnly的，这样前端JavaScript可以访问
            options.Cookie.HttpOnly = false;
            // 设置Cookie只在HTTPS连接中传输
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            // 设置Cookie的SameSite属性为Strict，防止跨站请求伪造
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // 配置响应压缩
        builder.Services.AddResponseCompression(options =>
        {
            // 为HTTPS请求启用压缩
            options.EnableForHttps = true;
            // 添加Brotli压缩提供程序
            options.Providers.Add<BrotliCompressionProvider>();
            // 添加Gzip压缩提供程序
            options.Providers.Add<GzipCompressionProvider>();
        });

        // 配置Brotli压缩选项
        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            // 设置压缩级别为最优
            options.Level = CompressionLevel.Optimal;
        });

        // 配置Gzip压缩选项
        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            // 设置压缩级别为最优
            options.Level = CompressionLevel.Optimal;
        });

        // 注册响应缓存服务
        builder.Services.AddResponseCaching();

        // 注册分布式内存缓存服务
        builder.Services.AddDistributedMemoryCache();

        // 配置会话服务
        builder.Services.AddSession(options =>
        {
            // 设置会话空闲超时时间为30分钟
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            // 设置会话Cookie为HttpOnly
            options.Cookie.HttpOnly = true;
            // 设置会话Cookie为必要的
            options.Cookie.IsEssential = true;
            // 设置会话Cookie只在HTTPS连接中传输
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            // 设置会话Cookie的SameSite属性为Strict
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // 配置认证服务，使用Cookie认证
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                // 设置登录路径
                options.LoginPath = "/Admin/Login";
                // 设置Cookie过期时间为2小时
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                // 设置Cookie只在HTTPS连接中传输
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                // 设置Cookie的SameSite属性为Strict
                options.Cookie.SameSite = SameSiteMode.Strict;
                // 设置Cookie为HttpOnly
                options.Cookie.HttpOnly = true;
            });

        // 从环境变量或配置文件中获取数据库连接字符串
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                               ?? builder.Configuration.GetConnectionString("DefaultConnection");

        // 注册数据访问层服务
        builder.Services.AddDalServices(connectionString!);
        // 注册业务逻辑层服务
        builder.Services.AddBllServices(builder.Configuration);

        // 配置数据保护服务
        var dataProtectionBuilder = builder.Services.AddDataProtection()
            // 将密钥持久化到文件系统
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
            // 设置应用程序名称
            .SetApplicationName("FamilyTreeNew");

        // 如果是Windows操作系统
        if (OperatingSystem.IsWindows())
        {
            // 使用DPAPI保护密钥
            dataProtectionBuilder.ProtectKeysWithDpapi();
        }

        // 配置HSTS（HTTP Strict Transport Security）
        builder.Services.AddHsts(options =>
        {
            // 允许将此站点添加到浏览器的HSTS预加载列表
            options.Preload = true;
            // 包含子域名
            options.IncludeSubDomains = true;
            // HSTS的最大缓存时间为365天
            options.MaxAge = TimeSpan.FromDays(365);
        });

        // 构建Web应用程序
        var app = builder.Build();

        // 使用异常处理中间件，指定错误页面
        app.UseExceptionHandler("/Home/Error");

        // 如果不是开发环境
        if (!app.Environment.IsDevelopment())
        {
            // 使用HSTS
            app.UseHsts();
        }

        // 使用状态码页面中间件，指定状态码页面
        app.UseStatusCodePagesWithReExecute("/Home/StatusCodePage", "?code={0}");

        // 使用自定义中间件设置安全头
        app.Use(async (context, next) =>
        {
            // 设置X-Frame-Options头，防止点击劫持
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
            // 设置X-Content-Type-Options头，防止MIME类型嗅探
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            // 设置X-XSS-Protection头，启用XSS保护
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            // 设置Referrer-Policy头，控制引用信息的发送
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            // 设置Permissions-Policy头，限制浏览器功能的使用
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

            // 定义内容安全策略
            var cspPolicy = "default-src 'self'; " +
                            "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.font.im https://cdn.jsdelivr.net; " +
                            "font-src 'self' https://fonts.gstatic.com https://fonts.font.im https://cdn.jsdelivr.net; " +
                            "img-src 'self' data: blob:; " +
                            "connect-src 'self'; " +
                            "frame-ancestors 'self'; " +
                            "base-uri 'self'; " +
                            "form-action 'self'";

            // 设置Content-Security-Policy头
            context.Response.Headers[HeaderNames.ContentSecurityPolicy] = cspPolicy;

            // 调用下一个中间件
            await next();
        });

        // 使用HTTPS重定向中间件
        app.UseHttpsRedirection();
        // 如果不是开发环境
        if (!app.Environment.IsDevelopment())
        {
            // 使用响应压缩中间件
            app.UseResponseCompression();
        }
        // 使用响应缓存中间件
        app.UseResponseCaching();

        // 配置静态文件服务
        app.UseStaticFiles(new StaticFileOptions
        {
            // 准备响应时的回调
            OnPrepareResponse = ctx =>
            {
                // 获取响应头
                var headers = ctx.Context.Response.GetTypedHeaders();
                // 设置缓存控制头
                headers.CacheControl = new CacheControlHeaderValue
                {
                    // 允许公共缓存
                    Public = true,
                    // 最大缓存时间为365天
                    MaxAge = TimeSpan.FromDays(365)
                };
            }
        });

        // 使用路由中间件
        app.UseRouting();
        // 使用会话中间件
        app.UseSession();
        // 使用认证中间件
        app.UseAuthentication();
        // 使用授权中间件
        app.UseAuthorization();
        // 映射静态资源
        app.MapStaticAssets();

        // 配置默认路由
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        // 运行应用程序
        app.Run();
    }
}