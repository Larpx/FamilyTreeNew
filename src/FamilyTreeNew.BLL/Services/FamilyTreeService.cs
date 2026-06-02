using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 家谱服务实现
/// </summary>
public class FamilyTreeService : IFamilyTreeService
{
    private readonly IFamilyTreeRepository _familyTreeRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<FamilyTreeService> _logger;

    private static readonly string FamilyTreeListCacheKey = "FamilyTree_List_{0}_{1}_{2}_{3}_{4}";
    private static readonly string FamilyTreeDetailCacheKey = "FamilyTree_Detail_{0}";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    // 使用静态版本计数器确保所有服务实例（Scoped）的缓存失效一致性
    // 当任一实例调用 InvalidateCache 时，所有实例的下次查询都会因版本号变化而 miss 缓存
    private static long FamilyTreeListCacheVersion = 0;

    public FamilyTreeService(IFamilyTreeRepository familyTreeRepository, IMemoryCache memoryCache, ILogger<FamilyTreeService> logger)
    {
        _familyTreeRepository = familyTreeRepository;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<FamilyTreeDto>> GetPagedAsync(FamilyTreeQueryDto query)
    {
        var cacheKey = string.Format(
            FamilyTreeListCacheKey,
            Interlocked.Read(ref FamilyTreeListCacheVersion),
            query.PageIndex,
            query.PageSize,
            query.Keyword ?? "",
            query.IsEnabled?.ToString() ?? "");

        if (_memoryCache.TryGetValue(cacheKey, out PagedResult<FamilyTreeDto>? cachedResult) && cachedResult != null)
        {
            return cachedResult;
        }

        var (items, totalCount) = await _familyTreeRepository.GetPagedWithMemberCountAsync(
            query.PageIndex, query.PageSize, query.Keyword, query.IsEnabled);

        var familyTreeIds = items.Select(i => i.Id).ToList();
        var memberCounts = await _familyTreeRepository.GetMemberCountsAsync(familyTreeIds);

        var dtos = items.Select(item => MapToDto(item, memberCounts.GetValueOrDefault(item.Id, 0))).ToList();

        var result = new PagedResult<FamilyTreeDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };

        _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration,
            SlidingExpiration = TimeSpan.FromMinutes(2)
        });

        return result;
    }

    /// <inheritdoc/>
    public async Task<FamilyTreeDto?> GetByIdAsync(Guid id)
    {
        var cacheKey = string.Format(FamilyTreeDetailCacheKey, id);

        if (_memoryCache.TryGetValue(cacheKey, out FamilyTreeDto? cachedResult) && cachedResult != null)
        {
            return cachedResult;
        }

        var entity = await _familyTreeRepository.GetByIdAsync(id);
        if (entity == null) return null;

        var memberCount = await _familyTreeRepository.GetMemberCountAsync(id);
        var dto = MapToDto(entity, memberCount);

        _memoryCache.Set(cacheKey, dto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration,
            SlidingExpiration = TimeSpan.FromMinutes(2)
        });

        return dto;
    }

    /// <inheritdoc/>
    public async Task<FamilyTreeDto> CreateAsync(FamilyTreeCreateDto dto)
    {
        var entity = new FamilyTree
        {
            Name = dto.Name,
            Description = InputSanitizer.SanitizeHtml(dto.Description),
            RequireVerification = dto.RequireVerification,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _familyTreeRepository.InsertAsync(entity);
        InvalidateCache();
        _logger.LogInformation("创建家谱，ID: {FamilyTreeId}，名称: {Name}", entity.Id, dto.Name);
        return MapToDto(entity, 0);
    }

    /// <inheritdoc/>
    public async Task<FamilyTreeDto?> UpdateAsync(Guid id, FamilyTreeUpdateDto dto)
    {
        var entity = await _familyTreeRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = InputSanitizer.SanitizeHtml(dto.Description);
        entity.RequireVerification = dto.RequireVerification;
        entity.IsEnabled = dto.IsEnabled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _familyTreeRepository.UpdateAsync(entity);
        InvalidateCache(id);
        _logger.LogInformation("更新家谱，ID: {FamilyTreeId}", id);
        var memberCount = await _familyTreeRepository.GetMemberCountAsync(id);
        return MapToDto(entity, memberCount);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _familyTreeRepository.ExistsAsync(id)) return false;
        await _familyTreeRepository.DeleteAsync(id);
        InvalidateCache(id);
        _logger.LogInformation("删除家谱，ID: {FamilyTreeId}", id);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _familyTreeRepository.ExistsAsync(id);
    }

    /// <summary>
    /// 将实体映射为DTO
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="memberCount">成员数量</param>
    /// <returns>DTO对象</returns>
    private static FamilyTreeDto MapToDto(FamilyTree entity, int memberCount)
    {
        return new FamilyTreeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            RequireVerification = entity.RequireVerification,
            IsEnabled = entity.IsEnabled,
            UpdatedAt = entity.UpdatedAt,
            MemberCount = memberCount
        };
    }

    /// <inheritdoc/>
    public void InvalidateCache(Guid? familyTreeId = null)
    {
        Interlocked.Increment(ref FamilyTreeListCacheVersion);

        if (familyTreeId.HasValue)
        {
            var detailKey = string.Format(FamilyTreeDetailCacheKey, familyTreeId.Value);
            _memoryCache.Remove(detailKey);
        }
    }
}
