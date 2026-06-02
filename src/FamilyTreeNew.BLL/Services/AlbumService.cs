using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 相册服务实现
/// </summary>
public class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _albumRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly ILogger<AlbumService> _logger;

    public AlbumService(IAlbumRepository albumRepository, IPhotoRepository photoRepository, ILogger<AlbumService> logger)
    {
        _albumRepository = albumRepository;
        _photoRepository = photoRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<AlbumDto>> GetAlbumsAsync(AlbumQueryDto query)
    {
        var albums = await _albumRepository.GetPagedAsync(query.PageIndex, query.PageSize, query.FamilyTreeId, query.Keyword);
        var totalCount = await _albumRepository.GetCountAsync(query.FamilyTreeId, query.Keyword);

        var albumIds = albums.Select(a => a.Id).ToList();
        var allPhotos = await _photoRepository.GetByAlbumIdsAsync(albumIds);
        var photosByAlbum = allPhotos.GroupBy(p => p.AlbumId).ToDictionary(g => g.Key, g => g.ToList());

        var albumDtos = albums.Select(album =>
        {
            var photos = photosByAlbum.GetValueOrDefault(album.Id, new List<Photo>());
            var coverPhoto = photos.FirstOrDefault();

            return new AlbumDto
            {
                Id = album.Id,
                FamilyTreeId = album.FamilyTreeId,
                Name = album.Name,
                Description = album.Description,
                CreatedAt = album.CreatedAt,
                UpdatedAt = album.UpdatedAt,
                PhotoCount = photos.Count,
                CoverPhotoPath = coverPhoto?.ThumbnailPath ?? coverPhoto?.PhotoPath
            };
        }).ToList();

        return new PagedResult<AlbumDto>
        {
            Items = albumDtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<AlbumDetailDto?> GetAlbumByIdAsync(Guid id)
    {
        var album = await _albumRepository.GetByIdWithPhotosAsync(id);
        if (album == null)
        {
            return null;
        }

        var photos = await _photoRepository.GetByAlbumIdAsync(id);
        var photoDtos = photos.Select(PhotoMapper.ToDto).ToList();

        return new AlbumDetailDto
        {
            Id = album.Id,
            FamilyTreeId = album.FamilyTreeId,
            Name = album.Name,
            Description = album.Description,
            CreatedAt = album.CreatedAt,
            UpdatedAt = album.UpdatedAt,
            PhotoCount = photos.Count,
            CoverPhotoPath = photoDtos.FirstOrDefault()?.ThumbnailPath ?? photoDtos.FirstOrDefault()?.PhotoPath,
            Photos = photoDtos
        };
    }

    /// <inheritdoc/>
    public async Task<AlbumDto> CreateAlbumAsync(AlbumCreateDto dto)
    {
        var album = new Album
        {
            Id = Guid.NewGuid(),
            FamilyTreeId = dto.FamilyTreeId,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _albumRepository.InsertAsync(album);
        _logger.LogInformation("创建相册，ID: {AlbumId}，家谱: {FamilyTreeId}", album.Id, dto.FamilyTreeId);

        return new AlbumDto
        {
            Id = album.Id,
            FamilyTreeId = album.FamilyTreeId,
            Name = album.Name,
            Description = album.Description,
            CreatedAt = album.CreatedAt,
            PhotoCount = 0
        };
    }

    /// <inheritdoc/>
    public async Task<AlbumDto?> UpdateAlbumAsync(Guid id, AlbumUpdateDto dto)
    {
        var album = await _albumRepository.GetByIdAsync(id);
        if (album == null)
        {
            return null;
        }

        album.Name = dto.Name;
        album.Description = dto.Description;
        album.UpdatedAt = DateTime.UtcNow;

        await _albumRepository.UpdateAsync(album);
        _logger.LogInformation("更新相册，ID: {AlbumId}", id);

        var photos = await _photoRepository.GetByAlbumIdAsync(id);
        var coverPhoto = photos.FirstOrDefault();

        return new AlbumDto
        {
            Id = album.Id,
            FamilyTreeId = album.FamilyTreeId,
            Name = album.Name,
            Description = album.Description,
            CreatedAt = album.CreatedAt,
            UpdatedAt = album.UpdatedAt,
            PhotoCount = photos.Count,
            CoverPhotoPath = coverPhoto?.ThumbnailPath ?? coverPhoto?.PhotoPath
        };
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAlbumAsync(Guid id)
    {
        var exists = await _albumRepository.ExistsAsync(id);
        if (!exists)
        {
            return false;
        }

        var photos = await _photoRepository.GetByAlbumIdAsync(id);
        foreach (var photo in photos)
        {
            if (!string.IsNullOrEmpty(photo.PhotoPath))
            {
                FileHelper.DeleteWebFile(photo.PhotoPath);
            }
            if (!string.IsNullOrEmpty(photo.ThumbnailPath))
            {
                FileHelper.DeleteWebFile(photo.ThumbnailPath);
            }
        }

        await _photoRepository.DeleteByAlbumIdAsync(id);
        await _albumRepository.DeleteAsync(id);
        _logger.LogInformation("删除相册，ID: {AlbumId}，关联照片数: {PhotoCount}", id, photos.Count);

        return true;
    }
}
