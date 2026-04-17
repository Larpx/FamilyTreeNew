using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class SourceCitationService : ISourceCitationService
{
    private readonly ISourceCitationRepository _sourceCitationRepository;
    private readonly ISourceRepository _sourceRepository;

    public SourceCitationService(ISourceCitationRepository sourceCitationRepository,
        ISourceRepository sourceRepository)
    {
        _sourceCitationRepository = sourceCitationRepository;
        _sourceRepository = sourceRepository;
    }

    public async Task<List<SourceCitationResponseDto>> GetBySourceIdAsync(Guid sourceId)
    {
        var citations = await _sourceCitationRepository.GetBySourceIdAsync(sourceId);
        return citations.Select(MapToDto).ToList();
    }

    public async Task<List<SourceCitationResponseDto>> GetByTargetAsync(string targetType, Guid targetId)
    {
        var citations = await _sourceCitationRepository.GetByTargetAsync(targetType, targetId);
        return citations.Select(MapToDto).ToList();
    }

    public async Task<SourceCitationResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _sourceCitationRepository.GetByIdAsync(id);
        return entity != null ? await MapToDtoAsync(entity) : null;
    }

    public async Task<SourceCitationResponseDto> CreateAsync(SourceCitationCreateRequestDto dto)
    {
        var entity = new SourceCitation
        {
            SourceId = dto.SourceId,
            TargetType = dto.TargetType,
            TargetId = dto.TargetId,
            Note = dto.Note,
            CreatedAt = DateTime.Now
        };

        await _sourceCitationRepository.InsertAsync(entity);
        return await MapToDtoAsync(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _sourceCitationRepository.ExistsAsync(id)) return false;
        await _sourceCitationRepository.DeleteAsync(id);
        return true;
    }

    private async Task<SourceCitationResponseDto> MapToDtoAsync(SourceCitation entity)
    {
        var source = await _sourceRepository.GetByIdAsync(entity.SourceId);

        return new SourceCitationResponseDto
        {
            Id = entity.Id,
            SourceId = entity.SourceId,
            SourceTitle = source?.Title ?? string.Empty,
            TargetType = entity.TargetType,
            TargetId = entity.TargetId,
            Note = entity.Note,
            CreatedAt = entity.CreatedAt
        };
    }

    private SourceCitationResponseDto MapToDto(SourceCitation entity)
    {
        return new SourceCitationResponseDto
        {
            Id = entity.Id,
            SourceId = entity.SourceId,
            SourceTitle = entity.Source?.Title ?? string.Empty,
            TargetType = entity.TargetType,
            TargetId = entity.TargetId,
            Note = entity.Note,
            CreatedAt = entity.CreatedAt
        };
    }
}