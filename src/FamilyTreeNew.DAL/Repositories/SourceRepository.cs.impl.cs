using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 来源仓储实现。
/// 负责按来源类型筛选和获取启用中的来源记录。
/// </summary>
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