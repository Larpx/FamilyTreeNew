using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 家族成员管理控制器，提供成员的增删改查功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FamilyMembersController : ControllerBase
{
    private readonly IFamilyMemberService _memberService;
    private readonly IFamilyTreeService _familyTreeService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<FamilyMembersController> _logger;

    private static readonly string MemberListCacheKey = "Member_List_{0}_{1}_{2}_{3}_{4}_{5}_{6}";
    private static readonly string MemberDetailCacheKey = "Member_Detail_{0}";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public FamilyMembersController(IFamilyMemberService memberService, IFamilyTreeService familyTreeService, IMemoryCache memoryCache, ILogger<FamilyMembersController> logger)
    {
        _memberService = memberService;
        _familyTreeService = familyTreeService;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    /// <summary>
    /// 获取家族成员列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含家谱ID、分页、关键词和世代过滤</param>
    /// <returns>分页的成员列表</returns>
    /// <response code="200">成功获取成员列表</response>
    /// <response code="400">未指定家谱ID</response>
    [HttpGet]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "FamilyTreeId", "PageIndex", "PageSize", "Keyword", "Generation", "ParentId" })]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FamilyMemberDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FamilyMemberDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagedResult<FamilyMemberDto>>>> GetList([FromQuery] FamilyMemberQueryDto query)
    {
        try
        {
            if (!query.FamilyTreeId.HasValue)
            {
                return BadRequest(ApiResponse<PagedResult<FamilyMemberDto>>.Fail("请指定家谱ID"));
            }

            var cacheKey = string.Format(MemberListCacheKey, 
                query.FamilyTreeId, query.PageIndex, query.PageSize, 
                query.Keyword ?? "", query.Generation?.ToString() ?? "", 
                query.ParentId?.ToString() ?? "", "");

            if (_memoryCache.TryGetValue(cacheKey, out PagedResult<FamilyMemberDto>? cachedResult) && cachedResult != null)
            {
                return Ok(ApiResponse<PagedResult<FamilyMemberDto>>.Ok(cachedResult));
            }

            var result = await _memberService.GetPagedAsync(query);

            _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            });

            return Ok(ApiResponse<PagedResult<FamilyMemberDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<PagedResult<FamilyMemberDto>>.Fail("获取成员列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据ID获取成员详情
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>成员详情</returns>
    /// <response code="200">成功获取成员详情</response>
    /// <response code="404">成员不存在</response>
    [HttpGet("{id}")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyMemberDto>>> GetById(Guid id)
    {
        try
        {
            var cacheKey = string.Format(MemberDetailCacheKey, id);

            if (_memoryCache.TryGetValue(cacheKey, out FamilyMemberDto? cachedResult) && cachedResult != null)
            {
                return Ok(ApiResponse<FamilyMemberDto>.Ok(cachedResult));
            }

            var result = await _memberService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<FamilyMemberDto>.Fail("成员不存在"));
            }

            _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            });

            return Ok(ApiResponse<FamilyMemberDto>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<FamilyMemberDto>.Fail("获取成员详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 创建家族成员
    /// </summary>
    /// <param name="dto">成员创建数据</param>
    /// <returns>创建的成员信息</returns>
    /// <response code="201">成员创建成功</response>
    /// <response code="400">数据验证失败</response>
    /// <response code="404">家谱不存在</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyMemberDto>>> Create([FromBody] FamilyMemberCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<FamilyMemberDto>.Fail("数据验证失败", 400, errors));
            }

            if (!await _familyTreeService.ExistsAsync(dto.FamilyTreeId))
            {
                return NotFound(ApiResponse<FamilyMemberDto>.Fail("指定的家谱不存在"));
            }

            var result = await _memberService.CreateAsync(dto);
            InvalidateMemberCache(dto.FamilyTreeId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<FamilyMemberDto>.Ok(result, "成员创建成功"));
        }
        catch (ArgumentException)
        {
            return BadRequest(ApiResponse<FamilyMemberDto>.Fail("操作失败，请稍后重试"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<FamilyMemberDto>.Fail("创建成员失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新成员信息
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <param name="dto">成员更新数据</param>
    /// <returns>更新后的成员信息</returns>
    /// <response code="200">成员更新成功</response>
    /// <response code="400">数据验证失败</response>
    /// <response code="404">成员不存在</response>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyMemberDto>>> Update(Guid id, [FromBody] FamilyMemberUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<FamilyMemberDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _memberService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<FamilyMemberDto>.Fail("成员不存在"));
            }

            InvalidateMemberCache(result.FamilyTreeId, id);
            return Ok(ApiResponse<FamilyMemberDto>.Ok(result, "成员更新成功"));
        }
        catch (ArgumentException)
        {
            return BadRequest(ApiResponse<FamilyMemberDto>.Fail("操作失败，请稍后重试"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<FamilyMemberDto>.Fail("更新成员失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除成员
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">成员删除成功</response>
    /// <response code="400">成员有子成员，无法删除</response>
    /// <response code="404">成员不存在</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound(ApiResponse.Fail("成员不存在"));
            }

            var result = await _memberService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("成员不存在"));
            }

            InvalidateMemberCache(member.FamilyTreeId, id);
            return Ok(ApiResponse.Ok("成员删除成功"));
        }
        catch (InvalidOperationException)
        {
            return BadRequest(ApiResponse.Fail("操作失败，请稍后重试"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.Fail("删除成员失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 使成员缓存失效
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <param name="memberId">成员ID（可选）</param>
    private void InvalidateMemberCache(Guid familyTreeId, Guid? memberId = null)
    {
        if (memberId.HasValue)
        {
            var detailKey = string.Format(MemberDetailCacheKey, memberId.Value);
            _memoryCache.Remove(detailKey);
        }
    }
}
