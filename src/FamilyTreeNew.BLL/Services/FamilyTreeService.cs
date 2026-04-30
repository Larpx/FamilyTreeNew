using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 家谱服务实现
/// </summary>
public class FamilyTreeService : IFamilyTreeService
{
    private readonly IFamilyTreeRepository _familyTreeRepository;

    public FamilyTreeService(IFamilyTreeRepository familyTreeRepository)
    {
        _familyTreeRepository = familyTreeRepository;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<FamilyTreeDto>> GetPagedAsync(FamilyTreeQueryDto query)
    {
        var (items, totalCount) = await _familyTreeRepository.GetPagedWithMemberCountAsync(
            query.PageIndex, query.PageSize, query.Keyword, query.IsEnabled);

        var familyTreeIds = items.Select(i => i.Id).ToList();
        var memberCounts = await _familyTreeRepository.GetMemberCountsAsync(familyTreeIds);

        var dtos = items.Select(item => MapToDto(item, memberCounts.GetValueOrDefault(item.Id, 0))).ToList();

        return new PagedResult<FamilyTreeDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<FamilyTreeDto?> GetByIdAsync(Guid id)
    {
        var entity = await _familyTreeRepository.GetByIdAsync(id);
        if (entity == null) return null;

        var memberCount = await _familyTreeRepository.GetMemberCountAsync(id);
        return MapToDto(entity, memberCount);
    }

    /// <inheritdoc/>
    public async Task<FamilyTreeDto> CreateAsync(FamilyTreeCreateDto dto)
    {
        var entity = new FamilyTree
        {
            Name = dto.Name,
            Description = dto.Description,
            RequireVerification = dto.RequireVerification,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _familyTreeRepository.InsertAsync(entity);
        return MapToDto(entity, 0);
    }

    /// <inheritdoc/>
    public async Task<FamilyTreeDto?> UpdateAsync(Guid id, FamilyTreeUpdateDto dto)
    {
        var entity = await _familyTreeRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.RequireVerification = dto.RequireVerification;
        entity.IsEnabled = dto.IsEnabled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _familyTreeRepository.UpdateAsync(entity);
        var memberCount = await _familyTreeRepository.GetMemberCountAsync(id);
        return MapToDto(entity, memberCount);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _familyTreeRepository.ExistsAsync(id)) return false;
        await _familyTreeRepository.DeleteAsync(id);
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
}
