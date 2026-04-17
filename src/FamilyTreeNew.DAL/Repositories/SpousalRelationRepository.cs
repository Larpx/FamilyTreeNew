using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 配偶关系仓储接口。
/// 用于按家谱、成员或配偶身份查询婚姻关系数据。
/// </summary>
public interface ISpousalRelationRepository : IBaseRepositoryGuid<SpousalRelation>
{
    Task<List<SpousalRelation>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    Task<List<SpousalRelation>> GetByMemberIdAsync(Guid memberId);

    Task<SpousalRelation?> GetByHusbandIdAsync(Guid husbandId);

    Task<SpousalRelation?> GetByWifeIdAsync(Guid wifeId);
}