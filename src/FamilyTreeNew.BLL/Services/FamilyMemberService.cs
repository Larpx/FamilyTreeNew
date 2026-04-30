using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 家族成员服务实现
/// </summary>
public class FamilyMemberService : IFamilyMemberService
{
    private readonly IFamilyMemberRepository _memberRepository;
    private readonly IFamilyTreeRepository _familyTreeRepository;

    public FamilyMemberService(IFamilyMemberRepository memberRepository, IFamilyTreeRepository familyTreeRepository)
    {
        _memberRepository = memberRepository;
        _familyTreeRepository = familyTreeRepository;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<FamilyMemberDto>> GetPagedAsync(FamilyMemberQueryDto query)
    {
        if (!query.FamilyTreeId.HasValue)
        {
            return new PagedResult<FamilyMemberDto>
            {
                Items = new List<FamilyMemberDto>(),
                TotalCount = 0,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }

        var (items, totalCount) = await _memberRepository.GetPagedByFamilyTreeAsync(
            query.FamilyTreeId.Value, query.PageIndex, query.PageSize, query.Keyword, query.Generation, query.ParentId);

        var parentIds = items
            .Where(i => i.ParentId.HasValue)
            .Select(i => i.ParentId!.Value)
            .Distinct()
            .ToList();

        var parentNames = await _memberRepository.GetParentNamesAsync(parentIds);

        var dtos = items.Select(item =>
        {
            var dto = MapToDto(item);
            if (item.ParentId.HasValue && parentNames.TryGetValue(item.ParentId.Value, out var parentName))
            {
                dto.ParentName = parentName;
            }
            return dto;
        }).ToList();

        return new PagedResult<FamilyMemberDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<FamilyMemberDto?> GetByIdAsync(Guid id)
    {
        var entity = await _memberRepository.GetByIdWithParentAsync(id);
        if (entity == null) return null;

        var dto = MapToDto(entity);
        if (entity.Parent != null)
        {
            dto.ParentName = $"{entity.Parent.Surname}{entity.Parent.FirstName}";
        }
        return dto;
    }

    /// <inheritdoc/>
    public async Task<FamilyMemberDto> CreateAsync(FamilyMemberCreateDto dto)
    {
        var generation = await CalculateGenerationAsync(dto.ParentId);

        var entity = new FamilyMember
        {
            FamilyTreeId = dto.FamilyTreeId,
            ParentId = dto.ParentId,
            Generation = generation,
            Surname = dto.Surname,
            FirstName = dto.FirstName,
            Alias = dto.Alias,
            Ranking = dto.Ranking,
            GenerationName = dto.GenerationName,
            Gender = dto.Gender,
            BirthDateSolar = dto.BirthDateSolar,
            BirthDateLunar = dto.BirthDateLunar,
            Residence = dto.Residence,
            Occupation = dto.Occupation,
            PersonalInfo = dto.PersonalInfo,
            Note = dto.Note,
            IsDeceased = dto.IsDeceased,
            DeathDateLunar = dto.DeathDateLunar,
            DeathDateSolar = dto.DeathDateSolar,
            Remarks = dto.Remarks,
            CreatedAt = DateTime.UtcNow
        };

        await _memberRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<FamilyMemberDto?> UpdateAsync(Guid id, FamilyMemberUpdateDto dto)
    {
        var entity = await _memberRepository.GetByIdAsync(id);
        if (entity == null) return null;

        if (entity.ParentId != dto.ParentId)
        {
            entity.Generation = await CalculateGenerationAsync(dto.ParentId);
        }

        entity.ParentId = dto.ParentId;
        entity.Surname = dto.Surname;
        entity.FirstName = dto.FirstName;
        entity.Alias = dto.Alias;
        entity.Ranking = dto.Ranking;
        entity.GenerationName = dto.GenerationName;
        entity.Gender = dto.Gender;
        entity.BirthDateSolar = dto.BirthDateSolar;
        entity.BirthDateLunar = dto.BirthDateLunar;
        entity.Residence = dto.Residence;
        entity.Occupation = dto.Occupation;
        entity.PersonalInfo = dto.PersonalInfo;
        entity.Note = dto.Note;
        entity.IsDeceased = dto.IsDeceased;
        entity.DeathDateLunar = dto.DeathDateLunar;
        entity.DeathDateSolar = dto.DeathDateSolar;
        entity.Remarks = dto.Remarks;
        entity.UpdatedAt = DateTime.UtcNow;

        await _memberRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _memberRepository.ExistsAsync(id)) return false;

        var hasChildren = await _memberRepository.HasChildrenAsync(id);
        if (hasChildren)
        {
            throw new InvalidOperationException("该成员有子成员，无法删除。请先删除或移动子成员。");
        }

        await _memberRepository.DeleteAsync(id);
        return true;
    }

    /// <inheritdoc/>
    public async Task<List<FamilyMemberDto>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var members = await _memberRepository.GetByFamilyTreeIdAsync(familyTreeId);
        return members.Select(m =>
        {
            var dto = MapToDto(m);
            if (m.Parent != null)
            {
                dto.ParentName = $"{m.Parent.Surname}{m.Parent.FirstName}";
            }
            return dto;
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<int> CalculateGenerationAsync(Guid? parentId)
    {
        if (!parentId.HasValue)
        {
            return 1;
        }

        var parentGeneration = await _memberRepository.GetGenerationByParentIdAsync(parentId.Value);
        if (!parentGeneration.HasValue)
        {
            throw new ArgumentException("指定的父成员不存在");
        }

        return parentGeneration.Value + 1;
    }

    /// <summary>
    /// 将实体映射为DTO
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns>DTO对象</returns>
    private static FamilyMemberDto MapToDto(FamilyMember entity)
    {
        return new FamilyMemberDto
        {
            Id = entity.Id,
            FamilyTreeId = entity.FamilyTreeId,
            ParentId = entity.ParentId,
            Generation = entity.Generation,
            Surname = entity.Surname,
            FirstName = entity.FirstName,
            Alias = entity.Alias,
            Ranking = entity.Ranking,
            GenerationName = entity.GenerationName,
            Gender = entity.Gender,
            BirthDateSolar = entity.BirthDateSolar,
            BirthDateLunar = entity.BirthDateLunar,
            Residence = entity.Residence,
            Occupation = entity.Occupation,
            PersonalInfo = entity.PersonalInfo,
            Note = entity.Note,
            IsDeceased = entity.IsDeceased,
            DeathDateLunar = entity.DeathDateLunar,
            DeathDateSolar = entity.DeathDateSolar,
            Remarks = entity.Remarks,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
