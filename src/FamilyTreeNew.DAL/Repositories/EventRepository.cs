using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 事件仓储接口。
/// 负责按家谱、成员、事件类型和地点查询事件数据。
/// </summary>
public interface IEventRepository : IBaseRepositoryGuid<Event>
{
    Task<List<Event>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    Task<List<Event>> GetByMemberIdAsync(Guid memberId);

    Task<List<Event>> GetByEventTypeIdAsync(Guid eventTypeId);

    Task<List<Event>> GetByPlaceIdAsync(Guid placeId);
}