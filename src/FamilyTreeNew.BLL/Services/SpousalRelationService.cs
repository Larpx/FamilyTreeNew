using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 配偶关系服务。
/// 负责管理婚姻关系的查询、创建、更新和删除。
/// </summary>
public class SpousalRelationService : ISpousalRelationService
{
    private readonly ISpousalRelationRepository _spousalRelationRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;

    public SpousalRelationService(ISpousalRelationRepository spousalRelationRepository,
        IFamilyMemberRepository familyMemberRepository)
    {
        _spousalRelationRepository = spousalRelationRepository;
        _familyMemberRepository = familyMemberRepository;
    }

    /// <summary>
    /// 获取某个家谱下的所有配偶关系。
    /// </summary>
    public async Task<List<SpousalRelationResponseDto>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var relations = await _spousalRelationRepository.GetByFamilyTreeIdAsync(familyTreeId);
        return relations.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 获取某个成员相关的所有配偶关系。
    /// </summary>
    public async Task<List<SpousalRelationResponseDto>> GetByMemberIdAsync(Guid memberId)
    {
        var relations = await _spousalRelationRepository.GetByMemberIdAsync(memberId);
        return relations.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 根据 ID 获取配偶关系。
    /// </summary>
    public async Task<SpousalRelationResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _spousalRelationRepository.GetByIdAsync(id);
        if (entity == null) return null;

        var husband = await _familyMemberRepository.GetByIdAsync(entity.HusbandId);
        var wife = await _familyMemberRepository.GetByIdAsync(entity.WifeId);
        return MapToDtoWithMembers(entity, husband, wife);
    }

    /// <summary>
    /// 创建配偶关系。
    /// 会禁止丈夫和妻子为同一个人。
    /// </summary>
    public async Task<SpousalRelationResponseDto> CreateAsync(SpousalRelationCreateRequestDto dto)
    {
        if (dto.HusbandId == dto.WifeId)
        {
            throw new ArgumentException("丈夫和妻子不能是同一个人");
        }

        var entity = new SpousalRelation
        {
            FamilyTreeId = dto.FamilyTreeId,
            HusbandId = dto.HusbandId,
            WifeId = dto.WifeId,
            MarriageDateSolar = dto.MarriageDateSolar,
            MarriageDateLunar = dto.MarriageDateLunar,
            Status = dto.Status,
            IsDivorced = dto.IsDivorced,
            DivorceDateSolar = dto.DivorceDateSolar,
            DivorceDateLunar = dto.DivorceDateLunar,
            Remarks = dto.Remarks,
            CreatedAt = DateTime.UtcNow
        };

        await _spousalRelationRepository.InsertAsync(entity);

        var husband = await _familyMemberRepository.GetByIdAsync(entity.HusbandId);
        var wife = await _familyMemberRepository.GetByIdAsync(entity.WifeId);
        return MapToDtoWithMembers(entity, husband, wife);
    }

    /// <summary>
    /// 更新配偶关系。
    /// </summary>
    public async Task<SpousalRelationResponseDto?> UpdateAsync(Guid id, SpousalRelationUpdateRequestDto dto)
    {
        var entity = await _spousalRelationRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.MarriageDateSolar = dto.MarriageDateSolar;
        entity.MarriageDateLunar = dto.MarriageDateLunar;
        entity.Status = dto.Status;
        entity.IsDivorced = dto.IsDivorced;
        entity.DivorceDateSolar = dto.DivorceDateSolar;
        entity.DivorceDateLunar = dto.DivorceDateLunar;
        entity.Remarks = dto.Remarks;
        entity.UpdatedAt = DateTime.UtcNow;

        await _spousalRelationRepository.UpdateAsync(entity);

        var husband = await _familyMemberRepository.GetByIdAsync(entity.HusbandId);
        var wife = await _familyMemberRepository.GetByIdAsync(entity.WifeId);
        return MapToDtoWithMembers(entity, husband, wife);
    }

    /// <summary>
    /// 删除配偶关系。
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _spousalRelationRepository.ExistsAsync(id)) return false;
        await _spousalRelationRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 将配偶关系实体转换为 DTO。
    /// </summary>
    private static SpousalRelationResponseDto MapToDto(SpousalRelation entity)
    {
        return new SpousalRelationResponseDto
        {
            Id = entity.Id,
            FamilyTreeId = entity.FamilyTreeId,
            HusbandId = entity.HusbandId,
            HusbandName = entity.Husband != null ? $"{entity.Husband.Surname}{entity.Husband.FirstName}" : string.Empty,
            WifeId = entity.WifeId,
            WifeName = entity.Wife != null ? $"{entity.Wife.Surname}{entity.Wife.FirstName}" : string.Empty,
            MarriageDateSolar = entity.MarriageDateSolar,
            MarriageDateLunar = entity.MarriageDateLunar,
            Status = entity.Status,
            IsDivorced = entity.IsDivorced,
            DivorceDateSolar = entity.DivorceDateSolar,
            DivorceDateLunar = entity.DivorceDateLunar,
            Remarks = entity.Remarks,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// 将配偶关系实体和成员信息一起转换为 DTO。
    /// </summary>
    private static SpousalRelationResponseDto MapToDtoWithMembers(SpousalRelation entity, FamilyMember? husband, FamilyMember? wife)
    {
        return new SpousalRelationResponseDto
        {
            Id = entity.Id,
            FamilyTreeId = entity.FamilyTreeId,
            HusbandId = entity.HusbandId,
            HusbandName = husband != null ? $"{husband.Surname}{husband.FirstName}" : string.Empty,
            WifeId = entity.WifeId,
            WifeName = wife != null ? $"{wife.Surname}{wife.FirstName}" : string.Empty,
            MarriageDateSolar = entity.MarriageDateSolar,
            MarriageDateLunar = entity.MarriageDateLunar,
            Status = entity.Status,
            IsDivorced = entity.IsDivorced,
            DivorceDateSolar = entity.DivorceDateSolar,
            DivorceDateLunar = entity.DivorceDateLunar,
            Remarks = entity.Remarks,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
