using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs.Auth;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Models.Helpers;

namespace FamilyTreeNew.BLL.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? userAgent = null);
    Task<Admin?> GetAdminByIdAsync(Guid id);
    Task<(bool Success, string Message)> ChangePasswordAsync(Guid adminId, string oldPassword, string newPassword);
}

public class AuthService : IAuthService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IOperationLogRepository _operationLogRepository;
    private readonly IJwtHelper _jwtHelper;

    public AuthService(
        IAdminRepository adminRepository,
        IOperationLogRepository operationLogRepository,
        IJwtHelper jwtHelper)
    {
        _adminRepository = adminRepository;
        _operationLogRepository = operationLogRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? userAgent = null)
    {
        var admin = await _adminRepository.GetByUsernameAsync(request.Username);

        if (admin == null)
        {
            await LogOperationAsync(null, "登录", "认证", "登录失败：用户不存在", ipAddress, userAgent, false, "用户名不存在");
            return new LoginResponseDto
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        if (!admin.IsEnabled)
        {
            await LogOperationAsync(admin.Id, "登录", "认证", "登录失败：账户已禁用", ipAddress, userAgent, false, "账户已禁用");
            return new LoginResponseDto
            {
                Success = false,
                Message = "账户已被禁用，请联系管理员"
            };
        }

        if (string.IsNullOrEmpty(admin.PasswordSalt))
        {
            await LogOperationAsync(admin.Id, "登录", "认证", "登录失败：密码未设置", ipAddress, userAgent, false, "密码盐值为空");
            return new LoginResponseDto
            {
                Success = false,
                Message = "账户配置异常，请联系管理员"
            };
        }

        if (!PasswordHelper.VerifyPassword(request.Password, admin.Password, admin.PasswordSalt))
        {
            await LogOperationAsync(admin.Id, "登录", "认证", "登录失败：密码错误", ipAddress, userAgent, false, "密码验证失败");
            return new LoginResponseDto
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        var token = _jwtHelper.GenerateToken(admin.Id, admin.Username, admin.PermissionLevel);
        var tokenExpiration = _jwtHelper.GetTokenExpiration();

        await _adminRepository.UpdateLastLoginTimeAsync(admin.Id);
        await LogOperationAsync(admin.Id, "登录", "认证", "登录成功", ipAddress, userAgent, true);

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
                PermissionLevel = admin.PermissionLevel,
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

        var validationResult = PasswordValidator.Validate(newPassword, minLength: 8, requireUppercase: true,
            requireLowercase: true, requireDigit: true, requireSpecialChar: true);

        if (!validationResult.IsValid)
        {
            return (false, string.Join("；", validationResult.Errors));
        }

        var hashedPassword = PasswordHelper.HashPassword(newPassword, out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        await _adminRepository.UpdateAsync(admin);
        await LogOperationAsync(adminId, "修改密码", "认证", "密码修改成功", null, null, true);

        return (true, "密码修改成功");
    }

    private async Task LogOperationAsync(
        Guid? adminId,
        string operationType,
        string module,
        string? content,
        string? ipAddress,
        string? userAgent,
        bool isSuccess,
        string? errorMessage = null)
    {
        var log = new OperationLog
        {
            AdminId = adminId,
            OperationType = operationType,
            Module = module,
            Content = content,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            OperationTime = DateTime.Now
        };

        await _operationLogRepository.InsertAsync(log);
    }
}
