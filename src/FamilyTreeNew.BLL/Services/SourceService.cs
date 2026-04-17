using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 来源服务。
/// 负责来源资料的查询、创建、更新和删除，并控制删除前的引用校验。
/// </summary>
public class SourceService : ISourceService
{
    private readonly ISourceRepository _sourceRepository;
    private readonly ISourceCitationRepository _sourceCitationRepository;

    public SourceService(ISourceRepository sourceRepository, ISourceCitationRepository sourceCitationRepository)
    {
        _sourceRepository = sourceRepository;
        _sourceCitationRepository = sourceCitationRepository;
    }

    /// <summary>
    /// 获取全部来源资料。
    /// </summary>
    public async Task<List<SourceResponseDto>> GetAllAsync()
    {
        var entities = await _sourceRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 获取启用中的来源资料。
    /// </summary>
    public async Task<List<SourceResponseDto>> GetEnabledSourcesAsync()
    {
        var entities = await _sourceRepository.GetEnabledSourcesAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 按类型获取来源资料。
    /// </summary>
    public async Task<List<SourceResponseDto>> GetByTypeAsync(string type)
    {
        var entities = await _sourceRepository.GetByTypeAsync(type);
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 根据 ID 获取来源资料。
    /// </summary>
    public async Task<SourceResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _sourceRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 创建来源资料。
    /// </summary>
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
            CreatedAt = DateTime.UtcNow
        };

        await _sourceRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 更新来源资料。
    /// </summary>
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
        entity.UpdatedAt = DateTime.UtcNow;

        await _sourceRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 删除来源资料。
    /// 如果该来源已经被引用，则拒绝删除。
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _sourceRepository.ExistsAsync(id)) return false;

        var relatedCitations = await _sourceCitationRepository.GetBySourceIdAsync(id);
        if (relatedCitations.Count > 0)
        {
            throw new InvalidOperationException("该来源下存在关联引用，无法删除");
        }

        await _sourceRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 将来源实体转换为 DTO。
    /// </summary>
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