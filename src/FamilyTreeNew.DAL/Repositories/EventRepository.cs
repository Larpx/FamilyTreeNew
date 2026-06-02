using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IEventRepository : IBaseRepositoryGuid<Event>
{
    Task<List<Event>> GetByFamilyTreeIdAsync(Guid familyTreeId);
    
    Task<List<Event>> GetByMemberIdAsync(Guid memberId);
    
    Task<List<Event>> GetByEventTypeIdAsync(Guid eventTypeId);
    
    Task<List<Event>> GetByPlaceIdAsync(Guid placeId);

    /// <summary>
    /// 根据ID获取事件（包含事件类型、地点、成员等关联信息）
    /// </summary>
    Task<Event?> GetByIdWithDetailsAsync(Guid id);
}