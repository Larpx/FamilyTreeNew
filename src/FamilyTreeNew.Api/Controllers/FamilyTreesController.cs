using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 家谱管理控制器，提供家谱的增删改查和成员导入功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FamilyTreesController : ControllerBase
{
    private readonly IFamilyTreeService _familyTreeService;
    private readonly IFamilyMemberService _memberService;
    private readonly IExcelImportService _excelImportService;
    private readonly IMemoryCache _memoryCache;

    private static readonly string FamilyTreeListCacheKey = "FamilyTree_List_{0}_{1}_{2}_{3}_{4}";
    private static readonly string FamilyTreeDetailCacheKey = "FamilyTree_Detail_{0}";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private static long FamilyTreeListCacheVersion = 0;

    public FamilyTreesController(
        IFamilyTreeService familyTreeService,
        IFamilyMemberService memberService,
        IExcelImportService excelImportService,
        IMemoryCache memoryCache)
    {
        _familyTreeService = familyTreeService;
        _memberService = memberService;
        _excelImportService = excelImportService;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// 获取家谱列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含分页、关键词和状态过滤</param>
    /// <returns>分页的家谱列表</returns>
    /// <response code="200">成功获取家谱列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FamilyTreeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<FamilyTreeDto>>>> GetList([FromQuery] FamilyTreeQueryDto query)
    {
        try
        {
            var cacheKey = string.Format(
                FamilyTreeListCacheKey,
                FamilyTreeListCacheVersion,
                query.PageIndex,
                query.PageSize,
                query.Keyword ?? "",
                query.IsEnabled?.ToString() ?? "");
            
            if (_memoryCache.TryGetValue(cacheKey, out PagedResult<FamilyTreeDto>? cachedResult) && cachedResult != null)
            {
                return Ok(ApiResponse<PagedResult<FamilyTreeDto>>.Ok(cachedResult));
            }

            var result = await _familyTreeService.GetPagedAsync(query);
            
            _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            });
            
            return Ok(ApiResponse<PagedResult<FamilyTreeDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<PagedResult<FamilyTreeDto>>.Fail("获取家谱列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据ID获取家谱详情
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <returns>家谱详情</returns>
    /// <response code="200">成功获取家谱详情</response>
    /// <response code="404">家谱不存在</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FamilyTreeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FamilyTreeDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyTreeDto>>> GetById(Guid id)
    {
        try
        {
            var cacheKey = string.Format(FamilyTreeDetailCacheKey, id);
            
            if (_memoryCache.TryGetValue(cacheKey, out FamilyTreeDto? cachedResult) && cachedResult != null)
            {
                return Ok(ApiResponse<FamilyTreeDto>.Ok(cachedResult));
            }

            var result = await _familyTreeService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<FamilyTreeDto>.Fail("家谱不存在"));
            }
            
            _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            });
            
            return Ok(ApiResponse<FamilyTreeDto>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<FamilyTreeDto>.Fail("获取家谱详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 创建新家谱
    /// </summary>
    /// <param name="dto">家谱创建数据</param>
    /// <returns>创建的家谱信息</returns>
    /// <response code="201">家谱创建成功</response>
    /// <response code="400">数据验证失败</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<FamilyTreeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FamilyTreeDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<FamilyTreeDto>>> Create([FromBody] FamilyTreeCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<FamilyTreeDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _familyTreeService.CreateAsync(dto);
            InvalidateFamilyTreeCache();
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<FamilyTreeDto>.Ok(result, "家谱创建成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<FamilyTreeDto>.Fail("创建家谱失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新家谱信息
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="dto">家谱更新数据</param>
    /// <returns>更新后的家谱信息</returns>
    /// <response code="200">家谱更新成功</response>
    /// <response code="400">数据验证失败</response>
    /// <response code="404">家谱不存在</response>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<FamilyTreeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FamilyTreeDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<FamilyTreeDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyTreeDto>>> Update(Guid id, [FromBody] FamilyTreeUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<FamilyTreeDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _familyTreeService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<FamilyTreeDto>.Fail("家谱不存在"));
            }
            
            InvalidateFamilyTreeCache(id);
            return Ok(ApiResponse<FamilyTreeDto>.Ok(result, "家谱更新成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<FamilyTreeDto>.Fail("更新家谱失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除家谱
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">家谱删除成功</response>
    /// <response code="404">家谱不存在</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _familyTreeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("家谱不存在"));
            }
            
            InvalidateFamilyTreeCache(id);
            return Ok(ApiResponse.Ok("家谱删除成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.Fail("删除家谱失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 获取家谱成员列表（分页）
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="pageIndex">页码，默认1</param>
    /// <param name="pageSize">每页数量，默认20</param>
    /// <param name="keyword">搜索关键词</param>
    /// <param name="generation">世代过滤</param>
    /// <param name="parentId">父成员ID过滤</param>
    /// <returns>分页的成员列表</returns>
    /// <response code="200">成功获取成员列表</response>
    /// <response code="404">家谱不存在</response>
    [HttpGet("{id}/members")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id", "pageIndex", "pageSize", "keyword", "generation", "parentId" })]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FamilyMemberDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FamilyMemberDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResult<FamilyMemberDto>>>> GetMembers(
        Guid id,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] int? generation = null,
        [FromQuery] Guid? parentId = null)
    {
        try
        {
            if (!await _familyTreeService.ExistsAsync(id))
            {
                return NotFound(ApiResponse<PagedResult<FamilyMemberDto>>.Fail("家谱不存在"));
            }

            var query = new FamilyMemberQueryDto
            {
                FamilyTreeId = id,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Keyword = keyword,
                Generation = generation,
                ParentId = parentId
            };

            var result = await _memberService.GetPagedAsync(query);
            return Ok(ApiResponse<PagedResult<FamilyMemberDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<PagedResult<FamilyMemberDto>>.Fail("获取家谱成员失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 向家谱添加成员
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="dto">成员创建数据</param>
    /// <returns>创建的成员信息</returns>
    /// <response code="201">成员添加成功</response>
    /// <response code="400">数据验证失败</response>
    /// <response code="404">家谱不存在</response>
    [HttpPost("{id}/members")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyMemberDto>>> AddMember(Guid id, [FromBody] FamilyMemberCreateDto dto)
    {
        try
        {
            if (!await _familyTreeService.ExistsAsync(id))
            {
                return NotFound(ApiResponse<FamilyMemberDto>.Fail("家谱不存在"));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<FamilyMemberDto>.Fail("数据验证失败", 400, errors));
            }

            dto.FamilyTreeId = id;
            var result = await _memberService.CreateAsync(dto);
            InvalidateFamilyTreeCache(id);
            return CreatedAtAction(nameof(FamilyMembersController.GetById), "FamilyMembers", new { id = result.Id }, ApiResponse<FamilyMemberDto>.Ok(result, "成员添加成功"));
        }
        catch (ArgumentException)
        {
            return BadRequest(ApiResponse<FamilyMemberDto>.Fail("操作失败，请稍后重试"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<FamilyMemberDto>.Fail("添加成员失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 从Excel文件导入成员
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="file">Excel文件（.xlsx或.xls格式）</param>
    /// <returns>导入结果</returns>
    /// <response code="200">导入成功</response>
    /// <response code="400">文件格式错误或数据验证失败</response>
    /// <response code="404">家谱不存在</response>
    [HttpPost("{id}/import")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ExcelImportResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ExcelImportResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ExcelImportResultDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ExcelImportResultDto>>> ImportMembers(Guid id, IFormFile file)
    {
        try
        {
            if (!await _familyTreeService.ExistsAsync(id))
            {
                return NotFound(ApiResponse<ExcelImportResultDto>.Fail("家谱不存在"));
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<ExcelImportResultDto>.Fail("请上传Excel文件"));
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest(ApiResponse<ExcelImportResultDto>.Fail("请上传Excel文件（.xlsx或.xls格式）"));
            }

            using var stream = file.OpenReadStream();
            var result = await _excelImportService.ImportMembersFromExcelAsync(id, stream);

            if (result.Success)
            {
                InvalidateFamilyTreeCache(id);
                return Ok(ApiResponse<ExcelImportResultDto>.Ok(result, result.Message));
            }
            else
            {
                return BadRequest(ApiResponse<ExcelImportResultDto>.Fail(result.Message, 400, result.Errors.Select(e => $"行{e.RowNumber}: {e.ErrorMessage}").ToList()));
            }
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<ExcelImportResultDto>.Fail("导入失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 下载成员导入模板
    /// </summary>
    /// <returns>Excel模板文件</returns>
    /// <response code="200">返回模板文件</response>
    [HttpGet("template")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public IActionResult DownloadTemplate()
    {
        try
        {
            var template = _excelImportService.GenerateTemplate();
            return File(template, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "成员导入模板.xlsx");
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.Fail("生成模板失败，请稍后重试"));
        }
    }

    private void InvalidateFamilyTreeCache(Guid? familyTreeId = null)
    {
        Interlocked.Increment(ref FamilyTreeListCacheVersion);

        if (familyTreeId.HasValue)
        {
            var detailKey = string.Format(FamilyTreeDetailCacheKey, familyTreeId.Value);
            _memoryCache.Remove(detailKey);
        }
    }
}
