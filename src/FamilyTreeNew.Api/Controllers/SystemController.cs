using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

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

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    [HttpGet("database-status")]
    public async Task<ActionResult<ApiResponse<DatabaseStatusDto>>> GetDatabaseStatus()
    {
        try
        {
            var status = await _systemService.GetDatabaseStatusAsync();
            return Ok(ApiResponse<DatabaseStatusDto>.Ok(status, "数据库状态获取成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database status");
            return StatusCode(500, ApiResponse<DatabaseStatusDto>.Fail("获取数据库状态失败"));
        }
    }

    [HttpGet("settings")]
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
            _logger.LogError(ex, "Error getting system settings");
            return StatusCode(500, ApiResponse<SystemSettingsDto>.Fail("获取系统设置失败"));
        }
    }

    [HttpPut("settings")]
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
            _logger.LogError(ex, "Error updating system settings");
            return StatusCode(500, ApiResponse<SystemSettingsDto>.Fail("更新系统设置失败"));
        }
    }

    [HttpPost("backup")]
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
            _logger.LogError(ex, "Error creating backup");
            return StatusCode(500, ApiResponse<BackupDto>.Fail("创建备份失败"));
        }
    }

    [HttpGet("backups")]
    public ActionResult<ApiResponse<BackupListDto>> GetBackupList()
    {
        try
        {
            var result = _backupService.GetBackupList();
            return Ok(ApiResponse<BackupListDto>.Ok(result, "获取备份列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backup list");
            return StatusCode(500, ApiResponse<BackupListDto>.Fail("获取备份列表失败"));
        }
    }

    [HttpPost("restore")]
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
            _logger.LogError(ex, "Error restoring backup");
            return StatusCode(500, ApiResponse<RestoreDto>.Fail("恢复备份失败"));
        }
    }

    [HttpDelete("backups/{fileName}")]
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
            _logger.LogError(ex, "Error deleting backup");
            return StatusCode(500, ApiResponse.Fail("删除备份失败"));
        }
    }
}
