using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IPlaceService
{
    Task<List<PlaceResponseDto>> GetAllAsync();
    
    Task<List<PlaceResponseDto>> GetEnabledPlacesAsync();
    
    Task<List<PlaceResponseDto>> GetByProvinceAsync(string province);
    
    Task<List<PlaceResponseDto>> GetByCityAsync(string city);
    
    Task<PlaceResponseDto?> GetByIdAsync(Guid id);
    
    Task<PlaceResponseDto> CreateAsync(PlaceCreateRequestDto dto);
    
    Task<PlaceResponseDto?> UpdateAsync(Guid id, PlaceUpdateRequestDto dto);
    
    Task<bool> DeleteAsync(Guid id);
}