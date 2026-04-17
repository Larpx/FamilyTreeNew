using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

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
