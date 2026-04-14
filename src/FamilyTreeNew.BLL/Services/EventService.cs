using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IPlaceRepository _placeRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;

    public EventService(IEventRepository eventRepository, 
        IEventTypeRepository eventTypeRepository,
        IPlaceRepository placeRepository,
        IFamilyMemberRepository familyMemberRepository)
    {
        _eventRepository = eventRepository;
        _eventTypeRepository = eventTypeRepository;
        _placeRepository = placeRepository;
        _familyMemberRepository = familyMemberRepository;
    }

    public async Task<List<EventResponseDto>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var events = await _eventRepository.GetByFamilyTreeIdAsync(familyTreeId);
        return await MapToDtosAsync(events);
    }

    public async Task<List<EventResponseDto>> GetByMemberIdAsync(Guid memberId)
    {
        var events = await _eventRepository.GetByMemberIdAsync(memberId);
        return await MapToDtosAsync(events);
    }

    public async Task<EventResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _eventRepository.GetByIdAsync(id);
        return entity != null ? await MapToDtoAsync(entity) : null;
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
            Description = dto.Description,
            IsPrimary = dto.IsPrimary,
            Remarks = dto.Remarks,
            CreatedAt = DateTime.Now
        };

        await _eventRepository.InsertAsync(entity);
        return await MapToDtoAsync(entity);
    }

    public async Task<EventResponseDto?> UpdateAsync(Guid id, EventUpdateRequestDto dto)
    {
        var entity = await _eventRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.EventTypeId = dto.EventTypeId;
        entity.PlaceId = dto.PlaceId;
        entity.DateSolar = dto.DateSolar;
        entity.DateLunar = dto.DateLunar;
        entity.Description = dto.Description;
        entity.IsPrimary = dto.IsPrimary;
        entity.Remarks = dto.Remarks;
        entity.UpdatedAt = DateTime.Now;

        await _eventRepository.UpdateAsync(entity);
        return await MapToDtoAsync(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _eventRepository.ExistsAsync(id)) return false;
        await _eventRepository.DeleteAsync(id);
        return true;
    }

    private async Task<List<EventResponseDto>> MapToDtosAsync(List<Event> entities)
    {
        var dtos = new List<EventResponseDto>();
        foreach (var entity in entities)
        {
            dtos.Add(await MapToDtoAsync(entity));
        }
        return dtos;
    }

    private async Task<EventResponseDto> MapToDtoAsync(Event entity)
    {
        var eventType = await _eventTypeRepository.GetByIdAsync(entity.EventTypeId);
        var place = entity.PlaceId.HasValue ? await _placeRepository.GetByIdAsync(entity.PlaceId.Value) : null;
        var member = await _familyMemberRepository.GetByIdAsync(entity.MemberId);

        return new EventResponseDto
        {
            Id = entity.Id,
            EventTypeId = entity.EventTypeId,
            EventTypeName = eventType?.Name ?? string.Empty,
            FamilyTreeId = entity.FamilyTreeId,
            MemberId = entity.MemberId,
            MemberName = member != null ? $"{member.Surname}{member.FirstName}" : string.Empty,
            PlaceId = entity.PlaceId,
            PlaceName = place?.Name,
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