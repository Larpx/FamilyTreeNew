using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.DAL.Context;

namespace FamilyTreeNew.DAL.Repositories;

public class EventRepository : BaseRepositoryGuid<Event>, IEventRepository
{
    public EventRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<Event>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Queryable<Event>()
            .Where(e => e.FamilyTreeId == familyTreeId)
            .Include(e => e.EventType)
            .Include(e => e.Place)
            .ToListAsync();
    }

    public async Task<List<Event>> GetByMemberIdAsync(Guid memberId)
    {
        return await Db.Queryable<Event>()
            .Where(e => e.MemberId == memberId)
            .Include(e => e.EventType)
            .Include(e => e.Place)
            .ToListAsync();
    }

    public async Task<List<Event>> GetByEventTypeIdAsync(Guid eventTypeId)
    {
        return await Db.Queryable<Event>()
            .Where(e => e.EventTypeId == eventTypeId)
            .Include(e => e.EventType)
            .Include(e => e.Place)
            .ToListAsync();
    }

    public async Task<List<Event>> GetByPlaceIdAsync(Guid placeId)
    {
        return await Db.Queryable<Event>()
            .Where(e => e.PlaceId == placeId)
            .Include(e => e.EventType)
            .Include(e => e.Place)
            .ToListAsync();
    }
}