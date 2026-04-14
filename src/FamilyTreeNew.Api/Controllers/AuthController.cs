using System.Security.Claims;
using FamilyTreeNew.Api.Extensions;
using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 认证控制器，处理用户登录、登出和密码管理
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IOperationLogService _operationLogService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IOperationLogService operationLogService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _operationLogService = operationLogService;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求，包含用户名和密码</param>
    /// <returns>登录响应，包含JWT Token和用户信息</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponseDto
                {
                    Success = false,
                    Message = "请求数据验证失败"
                });
            }

            var ipAddress = HttpContext.GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _authService.LoginAsync(request, ipAddress, userAgent);

            if (result.Success)
            {
                _logger.LogInformation("管理员 {Username} 登录成功", request.Username);
                return Ok(result);
            }

            _logger.LogWarning("管理员 {Username} 登录失败：{Message}", request.Username, result.Message);
            return Unauthorized(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录过程中发生异常：{Message}", ex.Message);
            return StatusCode(500, new LoginResponseDto
            {
                Success = false,
                Message = "服务器内部错误，请稍后重试"
            });
        }
    }

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <returns>当前用户信息</returns>
    [Authorize]
    [HttpGet("info")]
    [ProducesResponseType(typeof(ApiResponse<AdminInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInfo()
    {
        try
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized(ApiResponse<object>.Fail("无效的身份信息", 401));
            }

            var admin = await _authService.GetAdminByIdAsync(adminId);
            if (admin == null)
            {
                return NotFound(ApiResponse<object>.Fail("管理员不存在", 404));
            }

            var adminInfo = new AdminInfoDto
            {
                Id = admin.Id,
                Username = admin.Username,
                PermissionLevel = admin.PermissionLevel,
                RealName = admin.RealName,
                Email = admin.Email
            };

            return Ok(ApiResponse<AdminInfoDto>.Ok(adminInfo, "获取管理员信息成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取管理员信息时发生异常：{Message}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Fail("服务器内部错误", 500));
        }
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request">修改密码请求，包含原密码和新密码</param>
    /// <returns>修改结果</returns>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("请求数据验证失败"));
            }

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized(ApiResponse<object>.Fail("无效的身份信息", 401));
            }

            var (success, message) = await _authService.ChangePasswordAsync(adminId, request.OldPassword, request.NewPassword);

            if (success)
            {
                await _operationLogService.LogAsync(
                    adminId, "修改密码", "认证", "密码修改成功",
                    HttpContext.GetClientIpAddress(), Request.Headers.UserAgent.ToString());

                return Ok(ApiResponse.Ok(message));
            }

            return BadRequest(ApiResponse.Fail(message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密码时发生异常：{Message}", ex.Message);
            return StatusCode(500, ApiResponse.Fail("服务器内部错误", 500));
        }
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <returns>登出结果</returns>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(adminIdClaim) && Guid.TryParse(adminIdClaim, out var adminId))
            {
                await _operationLogService.LogAsync(
                    adminId, "登出", "认证", "登出成功",
                    HttpContext.GetClientIpAddress(), Request.Headers.UserAgent.ToString());
            }

            _logger.LogInformation("管理员 {Username} 登出成功", username);
            return Ok(ApiResponse.Ok("登出成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出时发生异常：{Message}", ex.Message);
            return StatusCode(500, ApiResponse.Fail("服务器内部错误", 500));
        }
    }
}
