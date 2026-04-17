using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 事件类型服务。
/// 负责事件类型的查询、创建、更新和删除。
/// </summary>
public class EventTypeService : IEventTypeService
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IEventRepository _eventRepository;

    public EventTypeService(IEventTypeRepository eventTypeRepository, IEventRepository eventRepository)
    {
        _eventTypeRepository = eventTypeRepository;
        _eventRepository = eventRepository;
    }

    /// <summary>
    /// 获取全部事件类型。
    /// </summary>
    public async Task<List<EventTypeResponseDto>> GetAllAsync()
    {
        var entities = await _eventTypeRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 获取启用中的事件类型。
    /// </summary>
    public async Task<List<EventTypeResponseDto>> GetEnabledTypesAsync()
    {
        var entities = await _eventTypeRepository.GetEnabledTypesAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 根据 ID 获取事件类型。
    /// </summary>
    public async Task<EventTypeResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _eventTypeRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 根据编码获取事件类型。
    /// </summary>
    public async Task<EventTypeResponseDto?> GetByCodeAsync(string code)
    {
        var entity = await _eventTypeRepository.GetByCodeAsync(code);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 创建事件类型。
    /// 创建前会检查编码是否重复。
    /// </summary>
    public async Task<EventTypeResponseDto> CreateAsync(EventTypeCreateRequestDto dto)
    {
        var existingType = await _eventTypeRepository.GetByCodeAsync(dto.Code);
        if (existingType != null)
        {
            throw new InvalidOperationException($"事件类型编码 '{dto.Code}' 已存在");
        }

        var entity = new EventType
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            SortOrder = dto.SortOrder,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _eventTypeRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 更新事件类型。
    /// </summary>
    public async Task<EventTypeResponseDto?> UpdateAsync(Guid id, EventTypeUpdateRequestDto dto)
    {
        var entity = await _eventTypeRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.SortOrder = dto.SortOrder;
        entity.IsEnabled = dto.IsEnabled;

        await _eventTypeRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 删除事件类型。
    /// 如果该类型下还有事件，则拒绝删除。
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _eventTypeRepository.ExistsAsync(id)) return false;

        var relatedEvents = await _eventRepository.GetByEventTypeIdAsync(id);
        if (relatedEvents.Count > 0)
        {
            throw new InvalidOperationException("该事件类型下存在关联事件，无法删除");
        }

        await _eventTypeRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 将事件类型实体转换为 DTO。
    /// </summary>
    private static EventTypeResponseDto MapToDto(EventType entity)
    {
        return new EventTypeResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            SortOrder = entity.SortOrder,
            IsEnabled = entity.IsEnabled,
            CreatedAt = entity.CreatedAt
        };
    }
}