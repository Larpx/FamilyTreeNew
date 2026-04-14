using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IEventTypeRepository : IBaseRepositoryGuid<EventType>
{
    Task<EventType?> GetByCodeAsync(string code);
    
    Task<List<EventType>> GetEnabledTypesAsync();
}