using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 事件服务。
/// 负责事件的查询、创建、更新和删除，并在实体与 DTO 之间做转换。
/// </summary>
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

    /// <summary>
    /// 获取某个家谱下的所有事件。
    /// </summary>
    public async Task<List<EventResponseDto>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var events = await _eventRepository.GetByFamilyTreeIdAsync(familyTreeId);
        return events.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 获取某个成员的所有事件。
    /// </summary>
    public async Task<List<EventResponseDto>> GetByMemberIdAsync(Guid memberId)
    {
        var events = await _eventRepository.GetByMemberIdAsync(memberId);
        return events.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 根据 ID 获取事件。
    /// </summary>
    public async Task<EventResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _eventRepository.GetByIdAsync(id);
        if (entity == null) return null;

        var eventType = await _eventTypeRepository.GetByIdAsync(entity.EventTypeId);
        var place = entity.PlaceId.HasValue ? await _placeRepository.GetByIdAsync(entity.PlaceId.Value) : null;
        var member = await _familyMemberRepository.GetByIdAsync(entity.MemberId);
        return MapToDtoWithDetails(entity, eventType, place, member);
    }

    /// <summary>
    /// 创建事件。
    /// 会保存事件类型、成员、地点和时间等信息。
    /// </summary>
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
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.InsertAsync(entity);

        var eventType = await _eventTypeRepository.GetByIdAsync(entity.EventTypeId);
        var place = entity.PlaceId.HasValue ? await _placeRepository.GetByIdAsync(entity.PlaceId.Value) : null;
        var member = await _familyMemberRepository.GetByIdAsync(entity.MemberId);
        return MapToDtoWithDetails(entity, eventType, place, member);
    }

    /// <summary>
    /// 更新事件。
    /// </summary>
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
        entity.UpdatedAt = DateTime.UtcNow;

        await _eventRepository.UpdateAsync(entity);

        var eventType = await _eventTypeRepository.GetByIdAsync(entity.EventTypeId);
        var place = entity.PlaceId.HasValue ? await _placeRepository.GetByIdAsync(entity.PlaceId.Value) : null;
        var member = await _familyMemberRepository.GetByIdAsync(entity.MemberId);
        return MapToDtoWithDetails(entity, eventType, place, member);
    }

    /// <summary>
    /// 删除事件。
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _eventRepository.ExistsAsync(id)) return false;
        await _eventRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 将事件实体转换为 DTO。
    /// </summary>
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

    /// <summary>
    /// 将事件实体和关联对象一起转换为 DTO。
    /// </summary>
    private static EventResponseDto MapToDtoWithDetails(Event entity, EventType? eventType, Place? place, FamilyMember? member)
    {
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
