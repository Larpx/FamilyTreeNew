using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface ISpousalRelationService
{
    Task<List<SpousalRelationResponseDto>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    Task<List<SpousalRelationResponseDto>> GetByMemberIdAsync(Guid memberId);

    Task<SpousalRelationResponseDto?> GetByIdAsync(Guid id);

    Task<SpousalRelationResponseDto> CreateAsync(SpousalRelationCreateRequestDto dto);

    Task<SpousalRelationResponseDto?> UpdateAsync(Guid id, SpousalRelationUpdateRequestDto dto);

    Task<bool> DeleteAsync(Guid id);
}