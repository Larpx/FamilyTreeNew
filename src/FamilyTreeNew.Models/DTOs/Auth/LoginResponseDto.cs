namespace FamilyTreeNew.Models.DTOs.Auth;

/// <summary>
/// 登录响应DTO
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// 是否登录成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// JWT令牌
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 令牌过期时间
    /// </summary>
    public DateTime? TokenExpiration { get; set; }

    /// <summary>
    /// 管理员信息
    /// </summary>
    public AdminInfoDto? AdminInfo { get; set; }
}

/// <summary>
/// 管理员信息DTO
/// </summary>
public class AdminInfoDto
{
    /// <summary>
    /// 管理员ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 权限级别
    /// </summary>
    public int PermissionLevel { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string? Email { get; set; }
}
