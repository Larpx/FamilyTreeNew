using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Web.Controllers;

public abstract class AuthenticatedApiControllerBase : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    protected AuthenticatedApiControllerBase(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    protected HttpClient GetApiClient()
    {
        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        client.BaseAddress = new Uri(apiBaseUrl);

        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    protected async Task<IActionResult?> EnsureAuthenticatedAsync()
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            await SignOutAndClearSessionAsync();
            return RedirectToAction("Login", "Admin");
        }

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrWhiteSpace(token) || IsTokenExpired(token))
        {
            await SignOutAndClearSessionAsync();
            return RedirectToAction("Login", "Admin");
        }

        return null;
    }

    protected async Task<IActionResult?> HandleUnauthorizedResponseAsync(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return null;
        }

        await SignOutAndClearSessionAsync();
        return RedirectToAction("Login", "Admin");
    }

    protected static bool IsTokenExpired(string token)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.ValidTo <= DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    protected async Task SignOutAndClearSessionAsync()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
