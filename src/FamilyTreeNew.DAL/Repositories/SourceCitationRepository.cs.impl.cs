using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

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