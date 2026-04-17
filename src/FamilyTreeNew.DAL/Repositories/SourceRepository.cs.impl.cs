using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public class SourceRepository : BaseRepositoryGuid<Source>, ISourceRepository
{
    public SourceRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<Source>> GetByTypeAsync(string type)
    {
        return await Db.Queryable<Source>().Where(s => s.Type == type).ToListAsync();
    }

    public async Task<List<Source>> GetEnabledSourcesAsync()
    {
        return await Db.Queryable<Source>().Where(s => s.IsEnabled).ToListAsync();
    }
}