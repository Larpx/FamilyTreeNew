using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

public class SourceService : ISourceService
{
    private readonly ISourceRepository _sourceRepository;
    private readonly ISourceCitationRepository _sourceCitationRepository;
    private readonly ILogger<SourceService> _logger;

    public SourceService(ISourceRepository sourceRepository, ISourceCitationRepository sourceCitationRepository, ILogger<SourceService> logger)
    {
        _sourceRepository = sourceRepository;
        _sourceCitationRepository = sourceCitationRepository;
        _logger = logger;
    }

    public async Task<List<SourceResponseDto>> GetAllAsync()
    {
        var entities = await _sourceRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<SourceResponseDto>> GetEnabledSourcesAsync()
    {
        var entities = await _sourceRepository.GetEnabledSourcesAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<SourceResponseDto>> GetByTypeAsync(string type)
    {
        var entities = await _sourceRepository.GetByTypeAsync(type);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<SourceResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _sourceRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<SourceResponseDto> CreateAsync(SourceCreateRequestDto dto)
    {
        var entity = new Source
        {
            Title = dto.Title,
            Author = dto.Author,
            Publisher = dto.Publisher,
            Year = dto.Year,
            Url = dto.Url,
            Type = dto.Type,
            Description = InputSanitizer.SanitizeHtml(dto.Description),
            Citation = dto.Citation,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _sourceRepository.InsertAsync(entity);
        _logger.LogInformation("创建来源，ID: {SourceId}，标题: {Title}", entity.Id, dto.Title);
        return MapToDto(entity);
    }

    public async Task<SourceResponseDto?> UpdateAsync(Guid id, SourceUpdateRequestDto dto)
    {
        var entity = await _sourceRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Title = dto.Title;
        entity.Author = dto.Author;
        entity.Publisher = dto.Publisher;
        entity.Year = dto.Year;
        entity.Url = dto.Url;
        entity.Type = dto.Type;
        entity.Description = InputSanitizer.SanitizeHtml(dto.Description);
        entity.Citation = dto.Citation;
        entity.IsEnabled = dto.IsEnabled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _sourceRepository.UpdateAsync(entity);
        _logger.LogInformation("更新来源，ID: {SourceId}", id);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _sourceRepository.ExistsAsync(id)) return false;

        var relatedCitations = await _sourceCitationRepository.GetBySourceIdAsync(id);
        if (relatedCitations.Count > 0)
        {
            throw new InvalidOperationException("该来源下存在关联引用，无法删除");
        }

        await _sourceRepository.DeleteAsync(id);
        _logger.LogInformation("删除来源，ID: {SourceId}", id);
        return true;
    }

    private static SourceResponseDto MapToDto(Source entity)
    {
        return new SourceResponseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Author = entity.Author,
            Publisher = entity.Publisher,
            Year = entity.Year,
            Url = entity.Url,
            Type = entity.Type,
            Description = entity.Description,
            Citation = entity.Citation,
            IsEnabled = entity.IsEnabled,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}