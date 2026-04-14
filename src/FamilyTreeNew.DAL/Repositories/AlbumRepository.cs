using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 相册仓储接口，继承自IBaseRepositoryGuid，提供相册的查询、分页和关联验证功能
/// </summary>
public interface IAlbumRepository
{
    /// <summary>
    /// 根据ID获取相册
    /// </summary>
    /// <param name="id">相册ID</param>
    Task<Album?> GetByIdAsync(Guid id);

    /// <summary>
    /// 根据ID获取相册（包含照片列表）
    /// </summary>
    /// <param name="id">相册ID</param>
    Task<Album?> GetByIdWithPhotosAsync(Guid id);

    /// <summary>
    /// 根据家谱ID获取相册列表
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    Task<List<Album>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    /// <summary>
    /// 分页查询相册
    /// </summary>
    /// <param name="pageIndex">页码（从1开始）</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="familyTreeId">家谱ID过滤，可为null</param>
    /// <param name="keyword">关键词过滤，可为null</param>
    Task<List<Album>> GetPagedAsync(int pageIndex, int pageSize, Guid? familyTreeId, string? keyword);

    /// <summary>
    /// 获取相册总数
    /// </summary>
    /// <param name="familyTreeId">家谱ID过滤，可为null</param>
    /// <param name="keyword">关键词过滤，可为null</param>
    Task<int> GetCountAsync(Guid? familyTreeId, string? keyword);

    /// <summary>
    /// 插入相册
    /// </summary>
    /// <param name="album">待插入的相册实体</param>
    Task<int> InsertAsync(Album album);

    /// <summary>
    /// 更新相册
    /// </summary>
    /// <param name="album">待更新的相册实体</param>
    Task<int> UpdateAsync(Album album);

    /// <summary>
    /// 根据ID删除相册
    /// </summary>
    /// <param name="id">相册ID</param>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 判断指定ID的相册是否存在
    /// </summary>
    /// <param name="id">相册ID</param>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// 判断相册是否属于指定家谱
    /// </summary>
    /// <param name="id">相册ID</param>
    /// <param name="familyTreeId">家谱ID</param>
    Task<bool> ExistsInFamilyTreeAsync(Guid id, Guid familyTreeId);
}
