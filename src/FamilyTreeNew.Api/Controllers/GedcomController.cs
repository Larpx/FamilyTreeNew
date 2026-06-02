using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// GEDCOM控制器，提供GEDCOM格式的导入导出功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GedcomController : ControllerBase
{
    private readonly IGedcomService _gedcomService;
    private readonly ILogger<GedcomController> _logger;

    public GedcomController(IGedcomService gedcomService, ILogger<GedcomController> logger)
    {
        _gedcomService = gedcomService;
        _logger = logger;
    }

    /// <summary>
    /// 导出家谱为GEDCOM格式文件
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>GEDCOM格式文件</returns>
    [HttpGet("export/{familyTreeId}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Export(Guid familyTreeId)
    {
        try
        {
            var gedcomContent = await _gedcomService.ExportToGedcomAsync(familyTreeId);
            var bytes = System.Text.Encoding.UTF8.GetBytes(gedcomContent);
            return File(bytes, "text/plain", $"familytree_{familyTreeId}.ged");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导出家谱 {FamilyTreeId} 为GEDCOM格式失败", familyTreeId);
            return NotFound(ApiResponse.Fail("导出失败，未找到对应的家谱数据"));
        }
    }

    /// <summary>
    /// 从GEDCOM格式内容导入家谱数据
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <param name="dto">GEDCOM导入请求数据</param>
    /// <returns>导入结果</returns>
    [HttpPost("import/{familyTreeId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Import(Guid familyTreeId, [FromBody] GedcomImportRequestDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.Content))
            {
                return BadRequest(ApiResponse.Fail("GEDCOM内容不能为空"));
            }

            var result = await _gedcomService.ImportFromGedcomAsync(familyTreeId, dto.Content);

            if (result.Success)
            {
                return Ok(ApiResponse.Ok(result.Message));
            }
            else
            {
                return BadRequest(ApiResponse.Fail(result.Message));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导入GEDCOM数据到家谱 {FamilyTreeId} 失败", familyTreeId);
            return StatusCode(500, ApiResponse.Fail("导入失败，请稍后重试"));
        }
    }
}
