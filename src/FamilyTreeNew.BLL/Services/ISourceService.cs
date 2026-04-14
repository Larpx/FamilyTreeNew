using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface ISourceService
{
    Task<List<SourceResponseDto>> GetAllAsync();
    
    Task<List<SourceResponseDto>> GetEnabledSourcesAsync();
    
    Task<List<SourceResponseDto>> GetByTypeAsync(string type);
    
    Task<SourceResponseDto?> GetByIdAsync(Guid id);
    
    Task<SourceResponseDto> CreateAsync(SourceCreateRequestDto dto);
    
    Task<SourceResponseDto?> UpdateAsync(Guid id, SourceUpdateRequestDto dto);
    
    Task<bool> DeleteAsync(Guid id);
}