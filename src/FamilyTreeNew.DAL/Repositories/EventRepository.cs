using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IEventRepository : IBaseRepositoryGuid<Event>
{
    Task<List<Event>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    Task<List<Event>> GetByMemberIdAsync(Guid memberId);

    Task<List<Event>> GetByEventTypeIdAsync(Guid eventTypeId);

    Task<List<Event>> GetByPlaceIdAsync(Guid placeId);
}