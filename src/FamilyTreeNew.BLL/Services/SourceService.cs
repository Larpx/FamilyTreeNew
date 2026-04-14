using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class SourceService : ISourceService
{
    private readonly ISourceRepository _sourceRepository;

    public SourceService(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
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
            Description = dto.Description,
            Citation = dto.Citation,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.Now
        };

        await _sourceRepository.InsertAsync(entity);
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
        entity.Description = dto.Description;
        entity.Citation = dto.Citation;
        entity.IsEnabled = dto.IsEnabled;
        entity.UpdatedAt = DateTime.Now;

        await _sourceRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _sourceRepository.ExistsAsync(id)) return false;
        await _sourceRepository.DeleteAsync(id);
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