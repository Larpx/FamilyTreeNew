using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IVerificationQuestionRepository : IBaseRepositoryGuid<VerificationQuestion>
{
    Task<List<VerificationQuestion>> GetAllWithFamilyTreeAsync();
    Task<VerificationQuestion?> GetByIdWithFamilyTreeAsync(Guid id);
    Task<List<VerificationQuestion>> GetByFamilyTreeIdAsync(Guid familyTreeId);
    Task<int> DeleteByFamilyTreeIdAsync(Guid familyTreeId);
}

public class VerificationQuestionRepository : BaseRepositoryGuid<VerificationQuestion>, IVerificationQuestionRepository
{
    public VerificationQuestionRepository(SqlSugarContext context) : base(context)
    {
    }

    public async Task<List<VerificationQuestion>> GetAllWithFamilyTreeAsync()
    {
        return await Db.Queryable<VerificationQuestion>()
            .Includes(x => x.FamilyTree)
            .ToListAsync();
    }

    public async Task<VerificationQuestion?> GetByIdWithFamilyTreeAsync(Guid id)
    {
        return await Db.Queryable<VerificationQuestion>()
            .Includes(x => x.FamilyTree)
            .InSingleAsync(id);
    }

    public async Task<List<VerificationQuestion>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Queryable<VerificationQuestion>()
            .Where(x => x.FamilyTreeId == familyTreeId)
            .OrderBy(x => x.Order)
            .ToListAsync();
    }

    public async Task<int> DeleteByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Deleteable<VerificationQuestion>()
            .Where(x => x.FamilyTreeId == familyTreeId)
            .ExecuteCommandAsync();
    }
}
