using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 报告控制器，提供祖先报告、后裔报告和统计报告的生成功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// 获取祖先报告
    /// </summary>
    /// <param name="memberId">成员ID</param>
    /// <param name="generations">追溯代数，默认5代</param>
    /// <returns>祖先报告数据</returns>
    [HttpGet("ancestor/{memberId}")]
    [ProducesResponseType(typeof(ApiResponse<AncestorReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AncestorReportDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AncestorReportDto>>> GetAncestorReport(Guid memberId, [FromQuery] int generations = 5)
    {
        try
        {
            var result = await _reportService.GenerateAncestorReportAsync(memberId, generations);
            return Ok(ApiResponse<AncestorReportDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成祖先报告失败，成员ID: {MemberId}", memberId);
            return StatusCode(500, ApiResponse<AncestorReportDto>.Fail("生成祖先报告失败"));
        }
    }

    /// <summary>
    /// 获取后裔报告
    /// </summary>
    /// <param name="memberId">成员ID</param>
    /// <param name="generations">追溯代数，默认5代</param>
    /// <returns>后裔报告数据</returns>
    [HttpGet("descendant/{memberId}")]
    [ProducesResponseType(typeof(ApiResponse<DescendantReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DescendantReportDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DescendantReportDto>>> GetDescendantReport(Guid memberId, [FromQuery] int generations = 5)
    {
        try
        {
            var result = await _reportService.GenerateDescendantReportAsync(memberId, generations);
            return Ok(ApiResponse<DescendantReportDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成后裔报告失败，成员ID: {MemberId}", memberId);
            return StatusCode(500, ApiResponse<DescendantReportDto>.Fail("生成后裔报告失败"));
        }
    }

    /// <summary>
    /// 获取统计报告
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>统计报告数据</returns>
    [HttpGet("statistics/{familyTreeId}")]
    [ProducesResponseType(typeof(ApiResponse<StatisticsReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StatisticsReportDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StatisticsReportDto>>> GetStatisticsReport(Guid familyTreeId)
    {
        try
        {
            var result = await _reportService.GenerateStatisticsReportAsync(familyTreeId);
            return Ok(ApiResponse<StatisticsReportDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成统计报告失败，家谱ID: {FamilyTreeId}", familyTreeId);
            return StatusCode(500, ApiResponse<StatisticsReportDto>.Fail("生成统计报告失败"));
        }
    }

    /// <summary>
    /// 获取HTML格式报告
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>HTML报告文件</returns>
    [HttpGet("html/{familyTreeId}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetHtmlReport(Guid familyTreeId)
    {
        try
        {
            var htmlContent = await _reportService.GenerateHtmlReportAsync(familyTreeId);
            var bytes = System.Text.Encoding.UTF8.GetBytes(htmlContent);
            return File(bytes, "text/html", $"report_{familyTreeId}.html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成HTML报告失败，家谱ID: {FamilyTreeId}", familyTreeId);
            return StatusCode(500, ApiResponse.Fail("生成HTML报告失败，请稍后重试"));
        }
    }
}
