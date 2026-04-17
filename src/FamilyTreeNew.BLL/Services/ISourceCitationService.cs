using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface ISourceCitationService
{
    Task<List<SourceCitationResponseDto>> GetBySourceIdAsync(Guid sourceId);

    Task<List<SourceCitationResponseDto>> GetByTargetAsync(string targetType, Guid targetId);

    Task<SourceCitationResponseDto?> GetByIdAsync(Guid id);

    Task<SourceCitationResponseDto> CreateAsync(SourceCitationCreateRequestDto dto);

    Task<bool> DeleteAsync(Guid id);
}