using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IEventService
{
    Task<List<EventResponseDto>> GetByFamilyTreeIdAsync(Guid familyTreeId);
    
    Task<List<EventResponseDto>> GetByMemberIdAsync(Guid memberId);
    
    Task<EventResponseDto?> GetByIdAsync(Guid id);
    
    Task<EventResponseDto> CreateAsync(EventCreateRequestDto dto);
    
    Task<EventResponseDto?> UpdateAsync(Guid id, EventUpdateRequestDto dto);
    
    Task<bool> DeleteAsync(Guid id);
}