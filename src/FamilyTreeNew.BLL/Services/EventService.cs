using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<List<EventResponseDto>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var events = await _eventRepository.GetByFamilyTreeIdAsync(familyTreeId);
        return events.Select(MapToDto).ToList();
    }

    public async Task<List<EventResponseDto>> GetByMemberIdAsync(Guid memberId)
    {
        var events = await _eventRepository.GetByMemberIdAsync(memberId);
        return events.Select(MapToDto).ToList();
    }

    public async Task<EventResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _eventRepository.GetByIdWithDetailsAsync(id);
        if (entity == null) return null;

        return MapToDto(entity);
    }

    public async Task<EventResponseDto> CreateAsync(EventCreateRequestDto dto)
    {
        var entity = new Event
        {
            EventTypeId = dto.EventTypeId,
            FamilyTreeId = dto.FamilyTreeId,
            MemberId = dto.MemberId,
            PlaceId = dto.PlaceId,
            DateSolar = dto.DateSolar,
            DateLunar = dto.DateLunar,
            Description = InputSanitizer.SanitizeHtml(dto.Description),
            IsPrimary = dto.IsPrimary,
            Remarks = dto.Remarks,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.InsertAsync(entity);
        _logger.LogInformation("创建事件，ID: {EventId}，家谱: {FamilyTreeId}", entity.Id, dto.FamilyTreeId);

        var entityWithDetails = await _eventRepository.GetByIdWithDetailsAsync(entity.Id);
        return MapToDto(entityWithDetails!);
    }

    public async Task<EventResponseDto?> UpdateAsync(Guid id, EventUpdateRequestDto dto)
    {
        var entity = await _eventRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.EventTypeId = dto.EventTypeId;
        entity.PlaceId = dto.PlaceId;
        entity.DateSolar = dto.DateSolar;
        entity.DateLunar = dto.DateLunar;
        entity.Description = InputSanitizer.SanitizeHtml(dto.Description);
        entity.IsPrimary = dto.IsPrimary;
        entity.Remarks = dto.Remarks;
        entity.UpdatedAt = DateTime.UtcNow;

        await _eventRepository.UpdateAsync(entity);
        _logger.LogInformation("更新事件，ID: {EventId}", id);

        var entityWithDetails = await _eventRepository.GetByIdWithDetailsAsync(id);
        return MapToDto(entityWithDetails!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _eventRepository.ExistsAsync(id)) return false;
        await _eventRepository.DeleteAsync(id);
        _logger.LogInformation("删除事件，ID: {EventId}", id);
        return true;
    }

    private static EventResponseDto MapToDto(Event entity)
    {
        return new EventResponseDto
        {
            Id = entity.Id,
            EventTypeId = entity.EventTypeId,
            EventTypeName = entity.EventType?.Name ?? string.Empty,
            FamilyTreeId = entity.FamilyTreeId,
            MemberId = entity.MemberId,
            MemberName = entity.Member != null ? $"{entity.Member.Surname}{entity.Member.FirstName}" : string.Empty,
            PlaceId = entity.PlaceId,
            PlaceName = entity.Place?.Name,
            DateSolar = entity.DateSolar,
            DateLunar = entity.DateLunar,
            Description = entity.Description,
            IsPrimary = entity.IsPrimary,
            Remarks = entity.Remarks,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
