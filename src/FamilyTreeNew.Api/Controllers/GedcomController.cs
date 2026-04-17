using System.ComponentModel.DataAnnotations;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GedcomController : ControllerBase
{
    private const int MaxGedcomContentLength = 5_000_000;
    private readonly IGedcomService _gedcomService;

    public GedcomController(IGedcomService gedcomService)
    {
        _gedcomService = gedcomService;
    }

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
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpPost("import/{familyTreeId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Import(Guid familyTreeId, [FromBody] GedcomImportRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .ToList();
                return BadRequest(ApiResponse.Fail("数据验证失败", 400, errors));
            }

            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest(ApiResponse.Fail("GEDCOM内容不能为空"));
            }

            if (dto.Content.Length > MaxGedcomContentLength)
            {
                return BadRequest(ApiResponse.Fail("GEDCOM内容过大，请控制在5MB以内"));
            }

            var result = await _gedcomService.ImportFromGedcomAsync(familyTreeId, dto.Content.Trim());

            if (result.Success)
            {
                return Ok(ApiResponse.Ok(result.Message));
            }
            else
            {
                return BadRequest(ApiResponse.Fail(result.Message));
            }
        }
        catch
        {
            return StatusCode(500, ApiResponse.Fail("导入失败，请稍后重试"));
        }
    }

    public class GedcomImportRequestDto
    {
        [Required(ErrorMessage = "GEDCOM内容不能为空")]
        public string Content { get; set; } = string.Empty;
    }
}