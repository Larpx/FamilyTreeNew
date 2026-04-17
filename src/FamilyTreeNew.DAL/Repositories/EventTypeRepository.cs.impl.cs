using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 事件类型仓储实现。
/// 负责按编码查找事件类型并获取启用中的事件类型列表。
/// </summary>
public class EventTypeRepository : BaseRepositoryGuid<EventType>, IEventTypeRepository
{
    public EventTypeRepository(SqlSugarContext context) : base(context) { }

    public async Task<EventType?> GetByCodeAsync(string code)
    {
        return await Db.Queryable<EventType>().Where(t => t.Code == code).FirstAsync();
    }

    public async Task<List<EventType>> GetEnabledTypesAsync()
    {
        return await Db.Queryable<EventType>()
            .Where(t => t.IsEnabled)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();
    }
}