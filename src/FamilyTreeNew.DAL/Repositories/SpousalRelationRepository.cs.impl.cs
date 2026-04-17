using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 配偶关系仓储实现。
/// 负责按家谱、成员或配偶身份查询婚姻关系数据。
/// </summary>
public class SpousalRelationRepository : BaseRepositoryGuid<SpousalRelation>, ISpousalRelationRepository
{
    public SpousalRelationRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<SpousalRelation>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Includes(sr => sr.Husband)
            .Includes(sr => sr.Wife)
            .Where(sr => sr.FamilyTreeId == familyTreeId)
            .ToListAsync();
    }

    public async Task<List<SpousalRelation>> GetByMemberIdAsync(Guid memberId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Includes(sr => sr.Husband)
            .Includes(sr => sr.Wife)
            .Where(sr => sr.HusbandId == memberId || sr.WifeId == memberId)
            .ToListAsync();
    }

    public async Task<SpousalRelation?> GetByHusbandIdAsync(Guid husbandId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Includes(sr => sr.Husband)
            .Includes(sr => sr.Wife)
            .Where(sr => sr.HusbandId == husbandId)
            .FirstAsync();
    }

    public async Task<SpousalRelation?> GetByWifeIdAsync(Guid wifeId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Includes(sr => sr.Husband)
            .Includes(sr => sr.Wife)
            .Where(sr => sr.WifeId == wifeId)
            .FirstAsync();
    }
}
