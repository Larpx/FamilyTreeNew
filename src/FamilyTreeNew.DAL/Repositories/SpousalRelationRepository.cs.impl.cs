using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.DAL.Context;

namespace FamilyTreeNew.DAL.Repositories;

public class SpousalRelationRepository : BaseRepositoryGuid<SpousalRelation>, ISpousalRelationRepository
{
    public SpousalRelationRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<SpousalRelation>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Where(sr => sr.FamilyTreeId == familyTreeId)
            .ToListAsync();
    }

    public async Task<List<SpousalRelation>> GetByMemberIdAsync(Guid memberId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Where(sr => sr.HusbandId == memberId || sr.WifeId == memberId)
            .ToListAsync();
    }

    public async Task<SpousalRelation?> GetByHusbandIdAsync(Guid husbandId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Where(sr => sr.HusbandId == husbandId)
            .FirstAsync();
    }

    public async Task<SpousalRelation?> GetByWifeIdAsync(Guid wifeId)
    {
        return await Db.Queryable<SpousalRelation>()
            .Where(sr => sr.WifeId == wifeId)
            .FirstAsync();
    }
}