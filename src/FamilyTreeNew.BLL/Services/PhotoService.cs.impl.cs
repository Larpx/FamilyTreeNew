using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 照片服务实现，使用SkiaSharp作为图像处理库
/// SkiaSharp是跨平台的高性能2D图形API，用于图片缩略图生成等图像处理操作
/// </summary>
public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IAlbumRepository _albumRepository;
    private readonly IFamilyMemberRepository _memberRepository;
    private readonly ILogger<PhotoService> _logger;

    /// <summary>
    /// 缩略图宽度
    /// </summary>
    private const int ThumbnailWidth = 300;

    /// <summary>
    /// 缩略图高度
    /// </summary>
    private const int ThumbnailHeight = 300;

    public PhotoService(IPhotoRepository photoRepository, IAlbumRepository albumRepository, IFamilyMemberRepository memberRepository, ILogger<PhotoService> logger)
    {
        _photoRepository = photoRepository;
        _albumRepository = albumRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<PhotoDto>> GetPhotosAsync(PhotoQueryDto query)
    {
        var photos = await _photoRepository.GetPagedAsync(query.PageIndex, query.PageSize, query.AlbumId, query.MemberId);
        var totalCount = await _photoRepository.GetCountAsync(query.AlbumId, query.MemberId);

        var photoDtos = photos.Select(PhotoMapper.ToDto).ToList();

        return new PagedResult<PhotoDto>
        {
            Items = photoDtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<PhotoDto?> GetPhotoByIdAsync(Guid id)
    {
        var photo = await _photoRepository.GetByIdAsync(id);
        return photo == null ? null : PhotoMapper.ToDto(photo);
    }

    /// <inheritdoc/>
    public async Task<List<PhotoDto>> UploadPhotosAsync(Guid albumId, List<PhotoUploadItem> files, PhotoUploadDto dto)
    {
        var albumExists = await _albumRepository.ExistsAsync(albumId);
        if (!albumExists)
        {
            throw new ArgumentException("相册不存在");
        }

        var uploadedPhotos = new List<PhotoDto>();
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "photos");
        var thumbnailDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thumbnails");

        Directory.CreateDirectory(uploadDir);
        Directory.CreateDirectory(thumbnailDir);

        foreach (var file in files)
        {
            var (isValid, errorMessage) = FileHelper.ValidateImageFile(file.FileName, file.Length);
            if (!isValid)
            {
                file.Stream?.Dispose();
                continue;
            }

            var inputStream = file.Stream;
            if (inputStream == null)
            {
                continue;
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var photoPath = Path.Combine(uploadDir, fileName);
            var relativePhotoPath = $"/uploads/photos/{fileName}";

            try
            {
                using (var fileStream = new FileStream(photoPath, FileMode.Create))
                {
                    await inputStream.CopyToAsync(fileStream);
                }
            }
            finally
            {
                inputStream.Dispose();
            }

            var thumbnailFileName = $"thumb_{fileName}";
            var thumbnailPath = Path.Combine(thumbnailDir, thumbnailFileName);
            var relativeThumbnailPath = $"/uploads/thumbnails/{thumbnailFileName}";

            try
            {
                await CreateThumbnailAsync(photoPath, thumbnailPath);
            }
            catch
            {
                relativeThumbnailPath = relativePhotoPath;
            }

            var photo = new Photo
            {
                Id = Guid.NewGuid(),
                AlbumId = albumId,
                PhotoPath = relativePhotoPath,
                ThumbnailPath = relativeThumbnailPath,
                Title = dto.Title ?? Path.GetFileNameWithoutExtension(file.FileName),
                Description = dto.Description,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = dto.UploadedBy
            };

            await _photoRepository.InsertAsync(photo);
            uploadedPhotos.Add(PhotoMapper.ToDto(photo));
        }

        return uploadedPhotos;
    }

    /// <inheritdoc/>
    public async Task<PhotoDto?> UpdatePhotoAsync(Guid id, PhotoUpdateDto dto)
    {
        var photo = await _photoRepository.GetByIdAsync(id);
        if (photo == null)
        {
            return null;
        }

        photo.Title = dto.Title ?? photo.Title;
        photo.Description = dto.Description ?? photo.Description;

        await _photoRepository.UpdateAsync(photo);
        return PhotoMapper.ToDto(photo);
    }

    /// <inheritdoc/>
    public async Task<bool> DeletePhotoAsync(Guid id)
    {
        var photo = await _photoRepository.GetByIdAsync(id);
        if (photo == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(photo.PhotoPath))
        {
            FileHelper.DeleteWebFile(photo.PhotoPath);
        }
        if (!string.IsNullOrEmpty(photo.ThumbnailPath) && photo.ThumbnailPath != photo.PhotoPath)
        {
            FileHelper.DeleteWebFile(photo.ThumbnailPath);
        }

        await _photoRepository.DeleteAsync(id);
        return true;
    }

    /// <inheritdoc/>
    public async Task<PhotoDto?> LinkMemberAsync(Guid photoId, LinkMemberDto dto)
    {
        var photo = await _photoRepository.GetByIdAsync(photoId);
        if (photo == null)
        {
            return null;
        }

        var memberExists = await _memberRepository.ExistsAsync(dto.MemberId);
        if (!memberExists)
        {
            throw new ArgumentException("成员不存在");
        }

        photo.MemberId = dto.MemberId;
        await _photoRepository.UpdateAsync(photo);

        if (dto.SetAsAvatar)
        {
            var member = await _memberRepository.GetByIdAsync(dto.MemberId);
            if (member != null)
            {
                member.UpdatedAt = DateTime.UtcNow;
                await _memberRepository.UpdateAsync(member);
            }
        }

        return PhotoMapper.ToDto(photo);
    }

    /// <summary>
    /// 创建缩略图
    /// 使用SkiaSharp的SKBitmap.Decode加载原图，通过Resize方法按比例缩放，
    /// 最后以JPEG格式（质量85）编码输出到目标路径
    /// </summary>
    /// <param name="sourcePath">源文件路径</param>
    /// <param name="targetPath">目标文件路径</param>
    private static async Task CreateThumbnailAsync(string sourcePath, string targetPath)
    {
        using var original = SKBitmap.Decode(sourcePath);
        if (original == null) return;

        var ratioX = (float)ThumbnailWidth / original.Width;
        var ratioY = (float)ThumbnailHeight / original.Height;
        var ratio = Math.Min(ratioX, ratioY);
        var newWidth = (int)(original.Width * ratio);
        var newHeight = (int)(original.Height * ratio);

        var resized = original.Resize(new SKImageInfo(newWidth, newHeight), new SKSamplingOptions(SKFilterMode.Linear));
        if (resized == null) return;

        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
        using var stream = File.Create(targetPath);
        await data.AsStream().CopyToAsync(stream);

        resized.Dispose();
    }
}
