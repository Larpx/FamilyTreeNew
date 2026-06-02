using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 系统管理控制器
/// 提供健康检查、数据库状态、系统设置和备份恢复功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdminRole")]
public class SystemController : ControllerBase
{
    private readonly ISystemService _systemService;
    private readonly ISystemSettingsService _settingsService;
    private readonly IBackupService _backupService;
    private readonly ILogger<SystemController> _logger;

    public SystemController(
        ISystemService systemService,
        ISystemSettingsService settingsService,
        IBackupService backupService,
        ILogger<SystemController> logger)
    {
        _systemService = systemService;
        _settingsService = settingsService;
        _backupService = backupService;
        _logger = logger;
    }

    /// <summary>
    /// 健康检查
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// 获取数据库状态
    /// </summary>
    [HttpGet("database-status")]
    [ProducesResponseType(typeof(ApiResponse<DatabaseStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DatabaseStatusDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<DatabaseStatusDto>>> GetDatabaseStatus()
    {
        try
        {
            var status = await _systemService.GetDatabaseStatusAsync();
            return Ok(ApiResponse<DatabaseStatusDto>.Ok(status, "数据库状态获取成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据库状态失败");
            return StatusCode(500, ApiResponse<DatabaseStatusDto>.Fail("获取数据库状态失败"));
        }
    }

    /// <summary>
    /// 获取系统设置
    /// </summary>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(ApiResponse<SystemSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SystemSettingsDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<SystemSettingsDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SystemSettingsDto>>> GetSettings()
    {
        try
        {
            var settings = await _settingsService.GetSettingsAsync();
            if (settings == null)
            {
                return NotFound(ApiResponse<SystemSettingsDto>.Fail("系统设置不存在"));
            }
            return Ok(ApiResponse<SystemSettingsDto>.Ok(settings, "获取系统设置成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统设置失败");
            return StatusCode(500, ApiResponse<SystemSettingsDto>.Fail("获取系统设置失败"));
        }
    }

    /// <summary>
    /// 更新系统设置
    /// </summary>
    [HttpPut("settings")]
    [ProducesResponseType(typeof(ApiResponse<SystemSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SystemSettingsDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SystemSettingsDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SystemSettingsDto>>> UpdateSettings([FromBody] UpdateSystemSettingsDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<SystemSettingsDto>.Fail("数据验证失败", 400, errors));
            }

            var settings = await _settingsService.UpdateSettingsAsync(dto);
            if (settings == null)
            {
                return StatusCode(500, ApiResponse<SystemSettingsDto>.Fail("更新系统设置失败"));
            }
            return Ok(ApiResponse<SystemSettingsDto>.Ok(settings, "更新系统设置成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新系统设置失败");
            return StatusCode(500, ApiResponse<SystemSettingsDto>.Fail("更新系统设置失败"));
        }
    }

    /// <summary>
    /// 创建数据库备份
    /// </summary>
    [HttpPost("backup")]
    [ProducesResponseType(typeof(ApiResponse<BackupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BackupDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<BackupDto>>> CreateBackup()
    {
        try
        {
            var result = await _backupService.CreateBackupAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(500, ApiResponse<BackupDto>.Fail("创建备份失败", 500, [result.ErrorMessage ?? "未知错误"]));
            }
            return Ok(ApiResponse<BackupDto>.Ok(result, "创建备份成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建备份失败");
            return StatusCode(500, ApiResponse<BackupDto>.Fail("创建备份失败"));
        }
    }

    /// <summary>
    /// 获取备份列表
    /// </summary>
    [HttpGet("backups")]
    [ProducesResponseType(typeof(ApiResponse<BackupListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BackupListDto>), StatusCodes.Status500InternalServerError)]
    public ActionResult<ApiResponse<BackupListDto>> GetBackupList()
    {
        try
        {
            var result = _backupService.GetBackupList();
            return Ok(ApiResponse<BackupListDto>.Ok(result, "获取备份列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取备份列表失败");
            return StatusCode(500, ApiResponse<BackupListDto>.Fail("获取备份列表失败"));
        }
    }

    /// <summary>
    /// 恢复数据库备份
    /// </summary>
    [HttpPost("restore")]
    [ProducesResponseType(typeof(ApiResponse<RestoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RestoreDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RestoreDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RestoreDto>>> RestoreBackup([FromBody] RestoreRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                return BadRequest(ApiResponse<RestoreDto>.Fail("备份文件名不能为空"));
            }

            var result = await _backupService.RestoreBackupAsync(request.FileName);
            if (!result.IsSuccess)
            {
                return StatusCode(500, ApiResponse<RestoreDto>.Fail("恢复备份失败", 500, [result.ErrorMessage ?? "未知错误"]));
            }
            return Ok(ApiResponse<RestoreDto>.Ok(result, "恢复备份成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "恢复备份失败");
            return StatusCode(500, ApiResponse<RestoreDto>.Fail("恢复备份失败"));
        }
    }

    /// <summary>
    /// 删除备份文件
    /// </summary>
    [HttpDelete("backups/{fileName}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public ActionResult<ApiResponse> DeleteBackup(string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest(ApiResponse.Fail("备份文件名不能为空"));
            }

            var result = _backupService.DeleteBackup(fileName);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("备份文件不存在或删除失败"));
            }
            return Ok(ApiResponse.Ok("删除备份成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除备份失败，文件名: {FileName}", fileName);
            return StatusCode(500, ApiResponse.Fail("删除备份失败"));
        }
    }
}
