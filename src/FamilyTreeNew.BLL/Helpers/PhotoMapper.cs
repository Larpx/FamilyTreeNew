using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Helpers;

/// <summary>
/// 照片实体与DTO映射辅助类，提供统一的Photo到PhotoDto映射逻辑
/// </summary>
public static class PhotoMapper
{
    /// <summary>
    /// 将Photo实体映射为PhotoDto
    /// </summary>
    /// <param name="photo">照片实体</param>
    /// <returns>照片DTO</returns>
    public static PhotoDto ToDto(Photo photo)
    {
        return new PhotoDto
        {
            Id = photo.Id,
            AlbumId = photo.AlbumId,
            MemberId = photo.MemberId,
            PhotoPath = photo.PhotoPath,
            ThumbnailPath = photo.ThumbnailPath,
            Title = photo.Title,
            Description = photo.Description,
            UploadedAt = photo.UploadedAt,
            UploadedBy = photo.UploadedBy,
            MemberName = photo.Member != null ? $"{photo.Member.Surname}{photo.Member.FirstName}" : null
        };
    }
}
