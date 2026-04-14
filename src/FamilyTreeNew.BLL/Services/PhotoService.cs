using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 照片服务接口
/// </summary>
public interface IPhotoService
{
    /// <summary>
    /// 分页查询照片
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页结果</returns>
    Task<PagedResult<PhotoDto>> GetPhotosAsync(PhotoQueryDto query);

    /// <summary>
    /// 根据ID获取照片详情
    /// </summary>
    /// <param name="id">照片ID</param>
    /// <returns>照片DTO</returns>
    Task<PhotoDto?> GetPhotoByIdAsync(Guid id);

    /// <summary>
    /// 上传照片
    /// </summary>
    /// <param name="albumId">相册ID</param>
    /// <param name="files">文件列表</param>
    /// <param name="dto">上传参数</param>
    /// <returns>上传的照片列表</returns>
    Task<List<PhotoDto>> UploadPhotosAsync(Guid albumId, List<PhotoUploadItem> files, PhotoUploadDto dto);

    /// <summary>
    /// 更新照片信息
    /// </summary>
    /// <param name="id">照片ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新后的照片DTO</returns>
    Task<PhotoDto?> UpdatePhotoAsync(Guid id, PhotoUpdateDto dto);

    /// <summary>
    /// 删除照片
    /// </summary>
    /// <param name="id">照片ID</param>
    /// <returns>删除成功返回true</returns>
    Task<bool> DeletePhotoAsync(Guid id);

    /// <summary>
    /// 关联照片到家族成员
    /// </summary>
    /// <param name="photoId">照片ID</param>
    /// <param name="dto">关联参数</param>
    /// <returns>更新后的照片DTO</returns>
    Task<PhotoDto?> LinkMemberAsync(Guid photoId, LinkMemberDto dto);
}

/// <summary>
/// 照片上传项
/// </summary>
public class PhotoUploadItem
{
    /// <summary>
    /// 文件流
    /// </summary>
    public Stream Stream { get; set; } = Stream.Null;

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long Length { get; set; }
}
