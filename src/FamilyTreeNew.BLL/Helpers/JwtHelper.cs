using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FamilyTreeNew.BLL.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FamilyTreeNew.BLL.Helpers;

/// <summary>
/// JWT令牌工具接口，提供令牌生成、验证和过期时间获取功能
/// </summary>
public interface IJwtHelper
{
    /// <summary>
    /// 生成JWT令牌
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="username">用户名</param>
    /// <param name="permissionLevel">权限等级</param>
    /// <returns>JWT令牌字符串</returns>
    string GenerateToken(Guid adminId, string username, int permissionLevel);

    /// <summary>
    /// 验证JWT令牌有效性
    /// </summary>
    /// <param name="token">待验证的JWT令牌</param>
    /// <returns>验证成功返回ClaimsPrincipal，失败返回null</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// 获取当前配置下的令牌过期时间
    /// </summary>
    /// <returns>令牌过期时间（UTC）</returns>
    DateTime GetTokenExpiration();
}

/// <summary>
/// JWT令牌工具实现，使用HMAC-SHA256签名算法，缓存签名密钥和验证参数以提高性能
/// </summary>
public class JwtHelper : IJwtHelper
{
    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtHelper(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = _signingKey,
            ClockSkew = TimeSpan.Zero
        };
    }

    /// <inheritdoc/>
    public string GenerateToken(Guid adminId, string username, int permissionLevel)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, adminId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString()),
            new Claim("PermissionLevel", permissionLevel.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: _signingCredentials
        );

        return _tokenHandler.WriteToken(token);
    }

    /// <inheritdoc/>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
    }
}
