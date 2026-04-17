using System.IO.Compression;
using FamilyTreeNew.BLL.Extensions;
using FamilyTreeNew.DAL.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using CacheControlHeaderValue = Microsoft.Net.Http.Headers.CacheControlHeaderValue;
using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

namespace FamilyTreeNew.Web;

/// <summary> // 说明这是 ASP.NET Core Web 应用的启动入口类型。
/// `Program` 类负责注册服务、配置中间件，并最终启动整个站点。 // 帮助初学者理解此类在应用启动过程中的职责。
/// </summary> // XML 注释结束。
public partial class Program // 声明应用的主入口类。
{ // `Program` 类主体开始。
    /// <summary> // 说明 `Main` 方法是程序真正开始执行的位置。
    /// 按顺序完成服务注册、管道配置和站点启动。 // 帮助初学者理解启动流程的整体结构。
    /// </summary> // XML 注释结束。
    /// <param name="args">命令行启动参数，通常由宿主环境传入。</param> // 说明方法参数的含义。
    public static void Main(string[] args) // 定义应用程序入口方法。
    { // `Main` 方法体开始。
        var builder = WebApplication.CreateBuilder(args); // 创建 Web 应用构建器，后续所有服务和配置都从这里开始。

        builder.Services.AddControllersWithViews(); // 注册 MVC 控制器和视图支持，让项目可以返回 Razor 视图页面。

        builder.Services.AddHttpClient(); // 注册 `HttpClientFactory`，供控制器安全地创建访问后端 API 的客户端。

        builder.Services.AddAntiforgery(options => // 注册防伪服务，防止跨站请求伪造攻击。
        { // 防伪配置代码块开始。
            options.HeaderName = "X-XSRF-TOKEN"; // 指定前端通过请求头传递防伪令牌时使用的名称。
            options.Cookie.Name = "XSRF-TOKEN"; // 指定浏览器中保存防伪令牌的 Cookie 名称。
            options.Cookie.HttpOnly = false; // 允许前端脚本读取防伪 Cookie，从而把令牌放到请求头里。
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 要求只在 HTTPS 下发送防伪 Cookie，提升安全性。
            options.Cookie.SameSite = SameSiteMode.Strict; // 限制跨站点请求自动携带该 Cookie，进一步降低 CSRF 风险。
        }); // 防伪配置结束。

        builder.Services.AddResponseCompression(options => // 注册响应压缩服务，减少 HTML、CSS、JS 等内容的传输体积。
        { // 响应压缩配置代码块开始。
            options.EnableForHttps = true; // 即使是 HTTPS 请求也启用压缩，以提升页面加载速度。
            options.Providers.Add<BrotliCompressionProvider>(); // 启用 Brotli 压缩算法，通常压缩率更高。
            options.Providers.Add<GzipCompressionProvider>(); // 启用 Gzip 压缩算法，兼容更多客户端。
        }); // 响应压缩配置结束。

        builder.Services.Configure<BrotliCompressionProviderOptions>(options => // 单独配置 Brotli 压缩参数。
        { // Brotli 配置代码块开始。
            options.Level = CompressionLevel.Optimal; // 使用更优压缩率的级别，在体积和 CPU 消耗之间做平衡。
        }); // Brotli 配置结束。

        builder.Services.Configure<GzipCompressionProviderOptions>(options => // 单独配置 Gzip 压缩参数。
        { // Gzip 配置代码块开始。
            options.Level = CompressionLevel.Optimal; // 使用较优压缩级别，兼顾性能和体积。
        }); // Gzip 配置结束。

        builder.Services.AddResponseCaching(); // 注册响应缓存服务，允许对满足条件的响应进行缓存。

        builder.Services.AddDistributedMemoryCache(); // 注册基于内存的分布式缓存，供 Session 等功能使用。

        builder.Services.AddSession(options => // 注册 Session 服务，用于在多个请求之间保存当前登录用户的状态数据。
        { // Session 配置代码块开始。
            options.IdleTimeout = TimeSpan.FromMinutes(30); // 设置 30 分钟无操作后 Session 过期。
            options.Cookie.HttpOnly = true; // 禁止前端脚本读取 Session Cookie，降低被脚本窃取的风险。
            options.Cookie.IsEssential = true; // 标记该 Cookie 为站点核心功能所必需。
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 要求只在 HTTPS 下发送 Session Cookie。
            options.Cookie.SameSite = SameSiteMode.Strict; // 限制跨站自动携带 Session Cookie，减少跨站风险。
        }); // Session 配置结束。

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // 注册基于 Cookie 的网页登录认证。
            .AddCookie(options => // 配置 Cookie 认证的详细行为。
            { // Cookie 认证配置代码块开始。
                options.LoginPath = "/Admin/Login"; // 当用户未登录访问受保护页面时，自动跳转到后台登录页。
                options.ExpireTimeSpan = TimeSpan.FromHours(2); // 设置认证 Cookie 的有效期为 2 小时。
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 要求认证 Cookie 只在 HTTPS 下发送。
                options.Cookie.SameSite = SameSiteMode.Strict; // 限制跨站自动携带认证 Cookie，降低被滥用的概率。
                options.Cookie.HttpOnly = true; // 禁止脚本直接读取认证 Cookie，提高安全性。
            }); // Cookie 认证配置结束。

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") // 优先从环境变量读取数据库连接串，方便 Docker 和部署环境注入配置。
                               ?? builder.Configuration.GetConnectionString("DefaultConnection"); // 如果环境变量不存在，再回退到配置文件中的默认连接串。

        builder.Services.AddDalServices(connectionString!); // 注册数据访问层所需的仓储、数据库上下文等服务。
        builder.Services.AddBllServices(builder.Configuration); // 注册业务逻辑层服务，让控制器可以通过依赖注入直接使用。

        builder.Services.AddDataProtection() // 注册数据保护服务，用于加密认证 Cookie、防伪令牌等敏感数据。
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys"))) // 把密钥保存到项目目录下，避免应用重启后旧 Cookie 失效。
            .SetApplicationName("FamilyTreeNew"); // 指定统一的应用名称，确保同一应用的多个实例可以共享密钥。

        builder.Services.AddHsts(options => // 注册 HSTS 配置，通知浏览器后续始终使用 HTTPS 访问站点。
        { // HSTS 配置代码块开始。
            options.Preload = true; // 允许站点参与浏览器的 HSTS 预加载列表。
            options.IncludeSubDomains = true; // 让所有子域名也强制使用 HTTPS。
            options.MaxAge = TimeSpan.FromDays(365); // 告诉浏览器在一年内都记住该 HTTPS 策略。
        }); // HSTS 配置结束。

        var app = builder.Build(); // 根据前面的服务配置创建真正可运行的 Web 应用实例。

        if (!app.Environment.IsDevelopment()) // 只有在非开发环境下才启用更严格的线上异常和 HSTS 策略。
        { // 非开发环境分支开始。
            app.UseExceptionHandler("/Home/Error"); // 发生未处理异常时跳转到统一错误页，避免把异常细节暴露给用户。
            app.UseHsts(); // 启用 HSTS 中间件，让浏览器强制使用 HTTPS。
        } // 非开发环境分支结束。

        app.Use(async (context, next) => // 注册一个内联中间件，统一附加常见安全响应头。
        { // 安全响应头中间件开始。
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN"; // 禁止其他站点随意以 iframe 嵌入当前页面，防止点击劫持。
            context.Response.Headers["X-Content-Type-Options"] = "nosniff"; // 禁止浏览器猜测 MIME 类型，降低内容嗅探风险。
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block"; // 启用旧版浏览器的反 XSS 保护模式。
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin"; // 控制跨站请求时 Referer 头暴露的信息范围。
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()"; // 默认禁止页面使用地理位置、麦克风和摄像头权限。

            var cspPolicy = "default-src 'self'; " + // 规定默认只允许加载本站自身资源。
                            "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " + // 允许脚本来自本站和指定 CDN，并保留当前页面依赖的内联脚本。
                            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.font.im https://cdn.jsdelivr.net; " + // 允许样式来自本站、字体服务和 CDN。
                            "font-src 'self' https://fonts.gstatic.com https://fonts.font.im https://cdn.jsdelivr.net; " + // 允许字体文件从受信任的字体源加载。
                            "img-src 'self' data: blob:; " + // 允许图片来自本站以及 `data:`、`blob:` 内联资源。
                            "connect-src 'self'; " + // 只允许 AJAX、WebSocket 等连接访问本站。
                            "frame-ancestors 'self'; " + // 只允许本站自身把页面嵌入到 frame 中。
                            "base-uri 'self'; " + // 限制 `<base>` 标签只能指向本站，防止基础路径被篡改。
                            "form-action 'self'"; // 只允许表单提交到本站，避免表单被引导到恶意地址。

            context.Response.Headers[HeaderNames.ContentSecurityPolicy] = cspPolicy; // 把上面拼好的 CSP 策略写入响应头。

            await next(); // 调用管道中的下一个中间件继续处理当前请求。
        }); // 安全响应头中间件结束。

        app.UseHttpsRedirection(); // 当用户通过 HTTP 访问时，自动重定向到 HTTPS。
        app.UseResponseCompression(); // 启用前面注册的响应压缩功能。
        app.UseResponseCaching(); // 启用响应缓存中间件。

        app.UseStaticFiles(new StaticFileOptions // 配置静态文件中间件，为 CSS、JS、图片等资源提供访问能力。
        { // 静态文件选项代码块开始。
            OnPrepareResponse = ctx => // 在返回静态文件前统一设置缓存头。
            { // 静态文件响应配置开始。
                var headers = ctx.Context.Response.GetTypedHeaders(); // 取得强类型响应头对象，便于安全地设置缓存信息。
                headers.CacheControl = new CacheControlHeaderValue // 创建 `Cache-Control` 响应头配置。
                { // `Cache-Control` 配置开始。
                    Public = true, // 表示该静态资源可以被浏览器和中间缓存公开缓存。
                    MaxAge = TimeSpan.FromDays(365) // 指定静态资源最长缓存一年，减少重复下载。
                }; // `Cache-Control` 配置结束。
            } // 静态文件响应配置结束。
        }); // 静态文件中间件配置结束。

        app.UseRouting(); // 启用路由匹配，为后续控制器分发请求做好准备。
        app.UseSession(); // 启用 Session 中间件，让每个请求都能读取和写入 Session。
        app.UseAuthentication(); // 启用认证中间件，解析当前请求中的登录身份。
        app.UseAuthorization(); // 启用授权中间件，根据权限规则决定是否允许访问。
        app.MapStaticAssets(); // 映射框架生成的静态资源端点。

        app.MapControllerRoute( // 注册默认 MVC 路由规则。
            name: "default", // 把该路由命名为 `default`，便于后续引用。
            pattern: "{controller=Home}/{action=Index}/{id?}") // 指定默认访问 `HomeController.Index`，并允许可选的 `id` 参数。
            .WithStaticAssets(); // 让该路由映射同时关联静态资源能力。

        app.Run(); // 启动 Web 应用并开始监听传入请求。
    } // `Main` 方法体结束。
} // `Program` 类主体结束。
