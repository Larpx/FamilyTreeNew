using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 照片响应DTO
/// </summary>
public class PhotoDto
{
    /// <summary>
    /// 照片ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 所属相册ID
    /// </summary>
    public Guid AlbumId { get; set; }

    /// <summary>
    /// 关联的家族成员ID（可选）
    /// </summary>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 照片存储路径
    /// </summary>
    public string PhotoPath { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图存储路径
    /// </summary>
    public string? ThumbnailPath { get; set; }

    /// <summary>
    /// 照片标题
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 照片描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 上传时间
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// 上传者用户名
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// 关联的成员姓名
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// 是否为成员头像
    /// </summary>
    public bool IsAvatar { get; set; }
}

/// <summary>
/// 照片上传请求DTO
/// </summary>
public class PhotoUploadDto
{
    /// <summary>
    /// 所属相册ID
    /// </summary>
    [Required(ErrorMessage = "相册ID不能为空")]
    public Guid AlbumId { get; set; }

    /// <summary>
    /// 照片标题
    /// </summary>
    [StringLength(200, ErrorMessage = "照片标题不能超过200个字符")]
    public string? Title { get; set; }

    /// <summary>
    /// 照片描述
    /// </summary>
    [StringLength(500, ErrorMessage = "照片描述不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 上传者用户名
    /// </summary>
    public string? UploadedBy { get; set; }
}

/// <summary>
/// 更新照片请求DTO
/// </summary>
public class PhotoUpdateDto
{
    /// <summary>
    /// 照片标题
    /// </summary>
    [StringLength(200, ErrorMessage = "照片标题不能超过200个字符")]
    public string? Title { get; set; }

    /// <summary>
    /// 照片描述
    /// </summary>
    [StringLength(500, ErrorMessage = "照片描述不能超过500个字符")]
    public string? Description { get; set; }
}

/// <summary>
/// 关联成员请求DTO
/// </summary>
public class LinkMemberDto
{
    /// <summary>
    /// 成员ID
    /// </summary>
    [Required(ErrorMessage = "成员ID不能为空")]
    public Guid MemberId { get; set; }

    /// <summary>
    /// 是否设为头像
    /// </summary>
    public bool SetAsAvatar { get; set; } = false;
}

/// <summary>
/// 照片查询请求DTO
/// </summary>
public class PhotoQueryDto
{
    /// <summary>
    /// 相册ID（可选）
    /// </summary>
    public Guid? AlbumId { get; set; }

    /// <summary>
    /// 成员ID（可选）
    /// </summary>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")]
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    [Range(1, 100, ErrorMessage = "每页大小必须在1-100之间")]
    public int PageSize { get; set; } = 20;
}
