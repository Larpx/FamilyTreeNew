using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs.Auth;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Models.Helpers;

namespace FamilyTreeNew.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IOperationLogService _operationLogService;
    private readonly IJwtHelper _jwtHelper;
    private readonly PasswordValidator _passwordValidator;

    public AuthService(
        IAdminRepository adminRepository,
        IOperationLogService operationLogService,
        IJwtHelper jwtHelper,
        PasswordValidator passwordValidator)
    {
        _adminRepository = adminRepository;
        _operationLogService = operationLogService;
        _jwtHelper = jwtHelper;
        _passwordValidator = passwordValidator;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? userAgent = null)
    {
        var admin = await _adminRepository.GetByUsernameAsync(request.Username);

        if (admin == null)
        {
            await _operationLogService.LogAsync(null, "登录", "认证", "登录失败：用户不存在", ipAddress, userAgent, false, "用户名不存在");
            return new LoginResponseDto
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        if (!admin.IsEnabled)
        {
            await _operationLogService.LogAsync(admin.Id, "登录", "认证", "登录失败：账户已禁用", ipAddress, userAgent, false, "账户已禁用");
            return new LoginResponseDto
            {
                Success = false,
                Message = "账户已被禁用，请联系管理员"
            };
        }

        if (string.IsNullOrEmpty(admin.PasswordSalt))
        {
            await _operationLogService.LogAsync(admin.Id, "登录", "认证", "登录失败：密码未设置", ipAddress, userAgent, false, "密码盐值为空");
            return new LoginResponseDto
            {
                Success = false,
                Message = "账户配置异常，请联系管理员"
            };
        }

        if (!PasswordHelper.VerifyPassword(request.Password, admin.Password, admin.PasswordSalt))
        {
            await _operationLogService.LogAsync(admin.Id, "登录", "认证", "登录失败：密码错误", ipAddress, userAgent, false, "密码验证失败");
            return new LoginResponseDto
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        var token = _jwtHelper.GenerateToken(admin.Id, admin.Username);
        var tokenExpiration = _jwtHelper.GetTokenExpiration();

        await _adminRepository.UpdateLastLoginTimeAsync(admin.Id);
        await _operationLogService.LogAsync(admin.Id, "登录", "认证", "登录成功", ipAddress, userAgent, true);

        return new LoginResponseDto
        {
            Success = true,
            Message = "登录成功",
            Token = token,
            TokenExpiration = tokenExpiration,
            AdminInfo = new AdminInfoDto
            {
                Id = admin.Id,
                Username = admin.Username,
                RealName = admin.RealName,
                Email = admin.Email
            }
        };
    }

    public async Task<Admin?> GetAdminByIdAsync(Guid id)
    {
        return await _adminRepository.GetByIdAsync(id);
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(Guid adminId, string oldPassword, string newPassword)
    {
        var admin = await _adminRepository.GetByIdAsync(adminId);
        if (admin == null)
        {
            return (false, "用户不存在");
        }

        if (string.IsNullOrEmpty(admin.PasswordSalt) ||
            !PasswordHelper.VerifyPassword(oldPassword, admin.Password, admin.PasswordSalt))
        {
            return (false, "原密码错误");
        }

        var validationResult = _passwordValidator.Validate(newPassword);

        if (!validationResult.IsValid)
        {
            return (false, string.Join("；", validationResult.Errors));
        }

        var hashedPassword = PasswordHelper.HashPassword(newPassword, out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        await _adminRepository.UpdateAsync(admin);
        await _operationLogService.LogAsync(adminId, "修改密码", "认证", "密码修改成功", null, null, true);

        return (true, "密码修改成功");
    }
}
