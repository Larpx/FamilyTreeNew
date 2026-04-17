using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 事件仓储实现。
/// 提供事件的数据库查询能力，并自动加载事件类型、地点和成员信息。
/// </summary>
public class EventRepository : BaseRepositoryGuid<Event>, IEventRepository
{
    public EventRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<Event>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Queryable<Event>()
            .Includes(e => e.EventType)
            .Includes(e => e.Place)
            .Includes(e => e.Member)
            .Where(e => e.FamilyTreeId == familyTreeId)
            .ToListAsync();
    }

    public async Task<List<Event>> GetByMemberIdAsync(Guid memberId)
    {
        return await Db.Queryable<Event>()
            .Includes(e => e.EventType)
            .Includes(e => e.Place)
            .Includes(e => e.Member)
            .Where(e => e.MemberId == memberId)
            .ToListAsync();
    }

    public async Task<List<Event>> GetByEventTypeIdAsync(Guid eventTypeId)
    {
        return await Db.Queryable<Event>()
            .Includes(e => e.EventType)
            .Includes(e => e.Place)
            .Includes(e => e.Member)
            .Where(e => e.EventTypeId == eventTypeId)
            .ToListAsync();
    }

    public async Task<List<Event>> GetByPlaceIdAsync(Guid placeId)
    {
        return await Db.Queryable<Event>()
            .Includes(e => e.EventType)
            .Includes(e => e.Place)
            .Includes(e => e.Member)
            .Where(e => e.PlaceId == placeId)
            .ToListAsync();
    }
}
