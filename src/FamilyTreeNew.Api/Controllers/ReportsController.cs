using FamilyTreeNew.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

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
            return NotFound(ApiResponse<AncestorReportDto>.Fail(ex.Message));
        }
    }

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
            return NotFound(ApiResponse<DescendantReportDto>.Fail(ex.Message));
        }
    }

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
            return NotFound(ApiResponse<StatisticsReportDto>.Fail(ex.Message));
        }
    }

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
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }
}