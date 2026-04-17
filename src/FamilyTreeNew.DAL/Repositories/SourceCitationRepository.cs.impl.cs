using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 来源引用仓储实现。
/// 负责查询某个来源的引用记录，以及按目标对象查找引用信息。
/// </summary>
public class SourceCitationRepository : BaseRepositoryGuid<SourceCitation>, ISourceCitationRepository
{
    public SourceCitationRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<SourceCitation>> GetBySourceIdAsync(Guid sourceId)
    {
        return await Db.Queryable<SourceCitation>()
            .Includes(sc => sc.Source)
            .Where(sc => sc.SourceId == sourceId)
            .ToListAsync();
    }

    public async Task<List<SourceCitation>> GetByTargetAsync(string targetType, Guid targetId)
    {
        return await Db.Queryable<SourceCitation>()
            .Includes(sc => sc.Source)
            .Where(sc => sc.TargetType == targetType && sc.TargetId == targetId)
            .ToListAsync();
    }
}