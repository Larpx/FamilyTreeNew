// 引入压缩相关的命名空间
using System.IO.Compression;
// 引入文本编码相关的命名空间
using System.Text;
// 引入自定义中间件
using FamilyTreeNew.Api.Middleware;
// 引入业务逻辑层的配置
using FamilyTreeNew.BLL.Configuration;
// 引入业务逻辑层的扩展方法
using FamilyTreeNew.BLL.Extensions;
// 引入数据访问层的扩展方法
using FamilyTreeNew.DAL.Extensions;
// 引入JWT认证相关的命名空间
using Microsoft.AspNetCore.Authentication.JwtBearer;
// 引入数据保护相关的命名空间
using Microsoft.AspNetCore.DataProtection;
// 引入响应压缩相关的命名空间
using Microsoft.AspNetCore.ResponseCompression;
// 引入身份验证令牌相关的命名空间
    using Microsoft.IdentityModel.Tokens;

// 定义命名空间
namespace FamilyTreeNew.Api;

// 定义Program类
public partial class Program
{
    // 程序的主入口方法
    public static void Main(string[] args)
    {
        // 创建Web应用程序构建器
        var builder = WebApplication.CreateBuilder(args);

        // 注册控制器服务
        builder.Services.AddControllers();

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

        // 注册API端点资源管理器
        builder.Services.AddEndpointsApiExplorer();
        // 注册Swagger生成器
        builder.Services.AddSwaggerGen();

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

        // 注册内存缓存服务
        builder.Services.AddMemoryCache();

        // 从配置中获取JWT设置
        var jwtSettings = builder.Configuration.GetJwtSettings();

        // 配置认证服务
        builder.Services.AddAuthentication(options =>
        {
            // 设置默认的认证方案为JWTBearer
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            // 设置默认的挑战方案为JWTBearer
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        // 添加JWTBearer认证
        .AddJwtBearer(options =>
        {
            // 配置令牌验证参数
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 验证颁发者
                ValidateIssuer = true,
                // 验证受众
                ValidateAudience = true,
                // 验证令牌有效期
                ValidateLifetime = true,
                // 验证签名密钥
                ValidateIssuerSigningKey = true,
                // 有效的颁发者
                ValidIssuer = jwtSettings.Issuer,
                // 有效的受众
                ValidAudience = jwtSettings.Audience,
                // 签名密钥
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                // 时钟偏差设置为0，确保令牌在过期时间后立即失效
                ClockSkew = TimeSpan.Zero
            };

            // 配置JWTBearer事件
            options.Events = new JwtBearerEvents
            {
                // 认证失败时的处理
                OnAuthenticationFailed = context =>
                {
                    // 如果是令牌过期异常
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        // 在响应头中添加令牌过期标记
                        context.Response.Headers.Append("Token-Expired", "true");
                    }

                    // 返回完成的任务
                    return Task.CompletedTask;
                }
            };
        });

        // 配置授权服务
        builder.Services.AddAuthorization(options =>
        {
            // 添加管理员角色策略，要求PermissionLevel为99
            options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("PermissionLevel", "99"));
            // 添加用户角色策略，要求用户已认证
            options.AddPolicy("RequireUserRole", policy => policy.RequireAuthenticatedUser());
        });

        // 配置数据保护服务
        builder.Services.AddDataProtection()
            // 将密钥持久化到文件系统
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
            // 设置应用程序名称
            .SetApplicationName("FamilyTreeNew");

        // 从环境变量或配置文件中获取数据库连接字符串
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                               ?? builder.Configuration.GetConnectionString("DefaultConnection");

        // 注册数据访问层服务
        builder.Services.AddDalServices(connectionString!);
        // 注册业务逻辑层服务
        builder.Services.AddBllServices(builder.Configuration);

        // 从配置中获取CORS设置
        var corsSettings = builder.Configuration.GetCorsSettings();

        // 配置CORS服务
        builder.Services.AddCors(options =>
        {
            // 添加名为SecureCorsPolicy的CORS策略
            options.AddPolicy("SecureCorsPolicy", policy =>
            {
                // 如果配置了允许的源
                if (corsSettings.AllowedOrigins.Length > 0)
                {
                    // 使用配置的允许源
                    policy.WithOrigins(corsSettings.AllowedOrigins);
                }
                else
                {
                    // 如果允许凭据
                    if (corsSettings.AllowCredentials)
                    {
                        // 使用本地开发环境的源
                        policy.WithOrigins("https://localhost:5001", "https://localhost:5002");
                    }
                    else
                    {
                        // 允许任何源
                        policy.AllowAnyOrigin();
                    }
                }

                // 如果允许凭据
                if (corsSettings.AllowCredentials)
                {
                    // 允许凭据
                    policy.AllowCredentials();
                }
                else
                {
                    // 不允许凭据
                    policy.DisallowCredentials();
                }

                // 配置允许的HTTP方法
                policy
                    .WithMethods(corsSettings.AllowedMethods)
                    // 配置允许的HTTP头
                    .WithHeaders(corsSettings.AllowedHeaders)
                    // 设置预检请求的最大缓存时间
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.PreflightMaxAgeSeconds));
            });
        });

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

        // 如果是开发环境
        if (app.Environment.IsDevelopment())
        {
            // 使用Swagger中间件
            app.UseSwagger();
            // 使用SwaggerUI中间件
            app.UseSwaggerUI();
        }
        else
        {
            // 在生产环境中使用HSTS
            app.UseHsts();
        }

        // 使用全局异常处理中间件
        app.UseGlobalExceptionHandler();
        // 使用安全头中间件
        app.UseSecurityHeaders();
        // 使用XSS保护中间件
        app.UseXssProtection();
        // 使用限流中间件
        app.UseRateLimiting();

        // 使用HTTPS重定向中间件
        app.UseHttpsRedirection();

        // 使用响应压缩中间件
        app.UseResponseCompression();

        // 使用响应缓存中间件
        app.UseResponseCaching();

        // 使用CORS中间件，应用SecureCorsPolicy策略
        app.UseCors("SecureCorsPolicy");

        // 使用认证中间件
        app.UseAuthentication();
        // 使用授权中间件
        app.UseAuthorization();

        // 映射控制器路由
        app.MapControllers();

        // 运行应用程序
        app.Run();
    }
}