using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IEventTypeService
{
    Task<List<EventTypeResponseDto>> GetAllAsync();
    
    Task<List<EventTypeResponseDto>> GetEnabledTypesAsync();
    
    Task<EventTypeResponseDto?> GetByIdAsync(Guid id);
    
    Task<EventTypeResponseDto?> GetByCodeAsync(string code);
    
    Task<EventTypeResponseDto> CreateAsync(EventTypeCreateRequestDto dto);
    
    Task<EventTypeResponseDto?> UpdateAsync(Guid id, EventTypeUpdateRequestDto dto);
    
    Task<bool> DeleteAsync(Guid id);
}