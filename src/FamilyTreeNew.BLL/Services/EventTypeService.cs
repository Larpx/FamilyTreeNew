using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class EventTypeService : IEventTypeService
{
    private readonly IEventTypeRepository _eventTypeRepository;

    public EventTypeService(IEventTypeRepository eventTypeRepository)
    {
        _eventTypeRepository = eventTypeRepository;
    }

    public async Task<List<EventTypeResponseDto>> GetAllAsync()
    {
        var entities = await _eventTypeRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<EventTypeResponseDto>> GetEnabledTypesAsync()
    {
        var entities = await _eventTypeRepository.GetEnabledTypesAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<EventTypeResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _eventTypeRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<EventTypeResponseDto?> GetByCodeAsync(string code)
    {
        var entity = await _eventTypeRepository.GetByCodeAsync(code);
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<EventTypeResponseDto> CreateAsync(EventTypeCreateRequestDto dto)
    {
        var entity = new EventType
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            SortOrder = dto.SortOrder,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.Now
        };

        await _eventTypeRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

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

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _eventTypeRepository.ExistsAsync(id)) return false;
        await _eventTypeRepository.DeleteAsync(id);
        return true;
    }

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