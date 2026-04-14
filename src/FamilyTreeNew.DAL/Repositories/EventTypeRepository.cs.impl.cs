using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.DAL.Context;

namespace FamilyTreeNew.DAL.Repositories;

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