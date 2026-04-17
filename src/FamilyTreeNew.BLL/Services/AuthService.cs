// 引入业务逻辑层的助手类
using FamilyTreeNew.BLL.Helpers;
// 引入数据访问层的仓储
using FamilyTreeNew.DAL.Repositories;
// 引入认证相关的DTO
using FamilyTreeNew.Models.DTOs.Auth;
// 引入实体类
using FamilyTreeNew.Models.Entities;
// 引入模型的助手类
using FamilyTreeNew.Models.Helpers;

// 定义命名空间
namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求DTO</param>
    /// <param name="ipAddress">客户端IP地址</param>
    /// <param name="userAgent">客户端用户代理</param>
    /// <returns>登录响应DTO</returns>
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? userAgent = null);
    
    /// <summary>
    /// 根据ID获取管理员信息
    /// </summary>
    /// <param name="id">管理员ID</param>
    /// <returns>管理员实体</returns>
    Task<Admin?> GetAdminByIdAsync(Guid id);
    
    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="oldPassword">旧密码</param>
    /// <param name="newPassword">新密码</param>
    /// <returns>操作结果和消息</returns>
    Task<(bool Success, string Message)> ChangePasswordAsync(Guid adminId, string oldPassword, string newPassword);
}

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    // 管理员仓储
    private readonly IAdminRepository _adminRepository;
    // 操作日志仓储
    private readonly IOperationLogRepository _operationLogRepository;
    // JWT助手
    private readonly IJwtHelper _jwtHelper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="adminRepository">管理员仓储</param>
    /// <param name="operationLogRepository">操作日志仓储</param>
    /// <param name="jwtHelper">JWT助手</param>
    public AuthService(
        IAdminRepository adminRepository,
        IOperationLogRepository operationLogRepository,
        IJwtHelper jwtHelper)
    {
        _adminRepository = adminRepository;
        _operationLogRepository = operationLogRepository;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求DTO</param>
    /// <param name="ipAddress">客户端IP地址</param>
    /// <param name="userAgent">客户端用户代理</param>
    /// <returns>登录响应DTO</returns>
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? userAgent = null)
    {
        // 根据用户名获取管理员
        var admin = await _adminRepository.GetByUsernameAsync(request.Username);

        // 如果管理员不存在
        if (admin == null)
        {
            // 记录登录失败日志
            await LogOperationAsync(null, "登录", "认证", "登录失败：用户不存在", ipAddress, userAgent, false, "用户名不存在");
            // 返回登录失败响应
            return new LoginResponseDto
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        // 如果账户已禁用
        if (!admin.IsEnabled)
        {
            // 记录登录失败日志
            await LogOperationAsync(admin.Id, "登录", "认证", "登录失败：账户已禁用", ipAddress, userAgent, false, "账户已禁用");
            // 返回登录失败响应
            return new LoginResponseDto
            {
                Success = false,
                Message = "账户已被禁用，请联系管理员"
            };
        }

        // 如果密码盐值为空
        if (string.IsNullOrEmpty(admin.PasswordSalt))
        {
            // 记录登录失败日志
            await LogOperationAsync(admin.Id, "登录", "认证", "登录失败：密码未设置", ipAddress, userAgent, false, "密码盐值为空");
            // 返回登录失败响应
            return new LoginResponseDto
            {
                Success = false,
                Message = "账户配置异常，请联系管理员"
            };
        }

        // 验证密码
        if (!PasswordHelper.VerifyPassword(request.Password, admin.Password, admin.PasswordSalt))
        {
            // 记录登录失败日志
            await LogOperationAsync(admin.Id, "登录", "认证", "登录失败：密码错误", ipAddress, userAgent, false, "密码验证失败");
            // 返回登录失败响应
            return new LoginResponseDto
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        // 生成JWT令牌
        var token = _jwtHelper.GenerateToken(admin.Id, admin.Username, admin.PermissionLevel);
        // 获取令牌过期时间
        var tokenExpiration = _jwtHelper.GetTokenExpiration();

        // 更新最后登录时间
        await _adminRepository.UpdateLastLoginTimeAsync(admin.Id);
        // 记录登录成功日志
        await LogOperationAsync(admin.Id, "登录", "认证", "登录成功", ipAddress, userAgent, true);

        // 返回登录成功响应
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

    /// <summary>
    /// 根据ID获取管理员信息
    /// </summary>
    /// <param name="id">管理员ID</param>
    /// <returns>管理员实体</returns>
    public async Task<Admin?> GetAdminByIdAsync(Guid id)
    {
        return await _adminRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="oldPassword">旧密码</param>
    /// <param name="newPassword">新密码</param>
    /// <returns>操作结果和消息</returns>
    public async Task<(bool Success, string Message)> ChangePasswordAsync(Guid adminId, string oldPassword, string newPassword)
    {
        // 根据ID获取管理员
        var admin = await _adminRepository.GetByIdAsync(adminId);
        if (admin == null)
        {
            return (false, "用户不存在");
        }

        // 验证旧密码
        if (string.IsNullOrEmpty(admin.PasswordSalt) ||
            !PasswordHelper.VerifyPassword(oldPassword, admin.Password, admin.PasswordSalt))
        {
            return (false, "原密码错误");
        }

        // 验证新密码强度
        var validationResult = PasswordValidator.Validate(newPassword, minLength: 8, requireUppercase: true,
            requireLowercase: true, requireDigit: true, requireSpecialChar: true);

        if (!validationResult.IsValid)
        {
            return (false, string.Join("；", validationResult.Errors));
        }

        // 哈希新密码
        var hashedPassword = PasswordHelper.HashPassword(newPassword, out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        // 更新管理员信息
        await _adminRepository.UpdateAsync(admin);
        // 记录密码修改日志
        await LogOperationAsync(adminId, "修改密码", "认证", "密码修改成功", null, null, true);

        return (true, "密码修改成功");
    }

    /// <summary>
    /// 记录操作日志
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="operationType">操作类型</param>
    /// <param name="module">模块</param>
    /// <param name="content">内容</param>
    /// <param name="ipAddress">IP地址</param>
    /// <param name="userAgent">用户代理</param>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="errorMessage">错误信息</param>
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
        // 创建操作日志
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

        // 插入操作日志
        await _operationLogRepository.InsertAsync(log);
    }
}