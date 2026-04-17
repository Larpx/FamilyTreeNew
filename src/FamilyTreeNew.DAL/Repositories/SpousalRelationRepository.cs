using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface ISpousalRelationRepository : IBaseRepositoryGuid<SpousalRelation>
{
    Task<List<SpousalRelation>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    Task<List<SpousalRelation>> GetByMemberIdAsync(Guid memberId);

    Task<SpousalRelation?> GetByHusbandIdAsync(Guid husbandId);

    Task<SpousalRelation?> GetByWifeIdAsync(Guid wifeId);
}