using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 验证问题仓储接口。
/// 负责获取带家谱信息的验证问题、按家谱查询以及批量删除相关问题。
/// </summary>
public interface IVerificationQuestionRepository : IBaseRepositoryGuid<VerificationQuestion>
{
    Task<List<VerificationQuestion>> GetAllWithFamilyTreeAsync();
    Task<VerificationQuestion?> GetByIdWithFamilyTreeAsync(Guid id);
    Task<List<VerificationQuestion>> GetByFamilyTreeIdAsync(Guid familyTreeId);
    Task<int> DeleteByFamilyTreeIdAsync(Guid familyTreeId);
}

/// <summary>
/// 验证问题仓储实现。
/// 提供验证问题的查询、关联加载和批量删除等功能。
/// </summary>
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
