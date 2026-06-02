using FamilyTreeNew.Models.DTOs.Auth;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 认证服务接口，提供管理员登录认证和密码管理功能
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 管理员登录
    /// </summary>
    /// <param name="request">登录请求数据</param>
    /// <param name="ipAddress">请求IP地址</param>
    /// <param name="userAgent">请求用户代理</param>
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? userAgent = null);

    /// <summary>
    /// 根据ID获取管理员信息
    /// </summary>
    /// <param name="id">管理员ID</param>
    Task<Admin?> GetAdminByIdAsync(Guid id);

    /// <summary>
    /// 修改管理员密码
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="oldPassword">原密码</param>
    /// <param name="newPassword">新密码</param>
    Task<(bool Success, string Message)> ChangePasswordAsync(Guid adminId, string oldPassword, string newPassword);
}
