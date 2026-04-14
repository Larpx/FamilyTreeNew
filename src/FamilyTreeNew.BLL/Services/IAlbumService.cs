using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 相册服务接口，提供相册的增删改查功能
/// </summary>
public interface IAlbumService
{
    /// <summary>
    /// 根据查询条件获取相册分页列表
    /// </summary>
    /// <param name="query">相册查询参数，包含分页、关键词和家谱ID过滤</param>
    Task<PagedResult<AlbumDto>> GetAlbumsAsync(AlbumQueryDto query);

    /// <summary>
    /// 根据ID获取相册详情（包含照片列表）
    /// </summary>
    /// <param name="id">相册ID</param>
    Task<AlbumDetailDto?> GetAlbumByIdAsync(Guid id);

    /// <summary>
    /// 创建新相册
    /// </summary>
    /// <param name="dto">相册创建数据</param>
    Task<AlbumDto> CreateAlbumAsync(AlbumCreateDto dto);

    /// <summary>
    /// 更新相册信息
    /// </summary>
    /// <param name="id">相册ID</param>
    /// <param name="dto">相册更新数据</param>
    Task<AlbumDto?> UpdateAlbumAsync(Guid id, AlbumUpdateDto dto);

    /// <summary>
    /// 删除相册
    /// </summary>
    /// <param name="id">相册ID</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteAlbumAsync(Guid id);
}
