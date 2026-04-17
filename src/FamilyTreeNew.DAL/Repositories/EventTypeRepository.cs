using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 事件类型仓储接口。
/// 负责按编码查找事件类型并获取所有启用的事件类型列表。
/// </summary>
public interface IEventTypeRepository : IBaseRepositoryGuid<EventType>
{
    Task<EventType?> GetByCodeAsync(string code);

    Task<List<EventType>> GetEnabledTypesAsync();
}