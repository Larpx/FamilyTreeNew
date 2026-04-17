using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 家族服务接口。
/// 定义家族分支的查询、创建、更新和删除能力。
/// </summary>
public interface IFamilyService
{
    /// <summary>
    /// 获取全部家族列表。
    /// </summary>
    Task<List<FamilyResponseDto>> GetAllFamiliesAsync();
    /// <summary>
    /// 根据 ID 获取家族。
    /// </summary>
    Task<FamilyResponseDto?> GetFamilyByIdAsync(int id);
    /// <summary>
    /// 创建家族。
    /// </summary>
    Task<FamilyResponseDto> CreateFamilyAsync(FamilyCreateRequestDto dto);
    /// <summary>
    /// 更新家族。
    /// </summary>
    Task<FamilyResponseDto?> UpdateFamilyAsync(int id, FamilyUpdateRequestDto dto);
    /// <summary>
    /// 删除家族。
    /// </summary>
    Task<bool> DeleteFamilyAsync(int id);
}

/// <summary>
/// 家族服务实现。
/// 负责把家族仓储的数据暴露给上层业务，并处理家族相关的业务规则。
/// </summary>
public class FamilyService : IFamilyService
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;

    public FamilyService(IFamilyRepository familyRepository, IFamilyMemberRepository familyMemberRepository)
    {
        _familyRepository = familyRepository;
        _familyMemberRepository = familyMemberRepository;
    }

    /// <summary>
    /// 获取全部家族。
    /// </summary>
    public async Task<List<FamilyResponseDto>> GetAllFamiliesAsync()
    {
        var families = await _familyRepository.GetFamiliesWithMemberCountAsync();
        return families.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 根据 ID 获取家族。
    /// </summary>
    public async Task<FamilyResponseDto?> GetFamilyByIdAsync(int id)
    {
        var family = await _familyRepository.GetByIdAsync(id);
        return family != null ? await MapToDtoAsync(family) : null;
    }

    /// <summary>
    /// 创建家族。
    /// </summary>
    public async Task<FamilyResponseDto> CreateFamilyAsync(FamilyCreateRequestDto dto)
    {
        var entity = new Family
        {
            FamilyTreeId = dto.FamilyTreeId,
            FamilyName = dto.FamilyName,
            HeadMemberId = dto.HeadMemberId,
            Address = dto.Address,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _familyRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 更新家族。
    /// </summary>
    public async Task<FamilyResponseDto?> UpdateFamilyAsync(int id, FamilyUpdateRequestDto dto)
    {
        var entity = await _familyRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.FamilyName = dto.FamilyName;
        entity.HeadMemberId = dto.HeadMemberId;
        entity.Address = dto.Address;
        entity.Description = dto.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        await _familyRepository.UpdateAsync(entity);
        return await MapToDtoAsync(entity);
    }

    /// <summary>
    /// 删除家族。
    /// </summary>
    public async Task<bool> DeleteFamilyAsync(int id)
    {
        var entity = await _familyRepository.GetByIdAsync(id);
        if (entity == null) return false;
        await _familyRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 将家族实体转换为 DTO。
    /// </summary>
    private static FamilyResponseDto MapToDto(Family entity)
    {
        return new FamilyResponseDto
        {
            Id = entity.Id,
            FamilyTreeId = entity.FamilyTreeId ?? Guid.Empty,
            FamilyName = entity.FamilyName,
            HeadMemberId = entity.HeadMemberId,
            Address = entity.Address,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// 将家族实体转换为 DTO，并补充负责人姓名。
    /// </summary>
    private async Task<FamilyResponseDto> MapToDtoAsync(Family entity)
    {
        var dto = MapToDto(entity);
        if (entity.HeadMemberId.HasValue)
        {
            var member = await _familyMemberRepository.GetByIdAsync(entity.HeadMemberId.Value);
            dto.HeadMemberName = member != null ? $"{member.Surname}{member.FirstName}" : null;
        }
        return dto;
    }
}
