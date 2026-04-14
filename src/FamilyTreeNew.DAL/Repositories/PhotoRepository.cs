using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 照片仓储接口，继承自IBaseRepositoryGuid，提供照片的查询、分页、批量插入和按相册删除功能
/// </summary>
public interface IPhotoRepository
{
    /// <summary>
    /// 根据ID获取照片
    /// </summary>
    /// <param name="id">照片ID</param>
    Task<Photo?> GetByIdAsync(Guid id);

    /// <summary>
    /// 根据相册ID获取照片列表
    /// </summary>
    /// <param name="albumId">相册ID</param>
    Task<List<Photo>> GetByAlbumIdAsync(Guid albumId);

    /// <summary>
    /// 根据成员ID获取照片列表
    /// </summary>
    /// <param name="memberId">成员ID</param>
    Task<List<Photo>> GetByMemberIdAsync(Guid memberId);

    /// <summary>
    /// 分页查询照片
    /// </summary>
    /// <param name="pageIndex">页码（从1开始）</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="albumId">相册ID过滤，可为null</param>
    /// <param name="memberId">成员ID过滤，可为null</param>
    Task<List<Photo>> GetPagedAsync(int pageIndex, int pageSize, Guid? albumId, Guid? memberId);

    /// <summary>
    /// 获取照片总数
    /// </summary>
    /// <param name="albumId">相册ID过滤，可为null</param>
    /// <param name="memberId">成员ID过滤，可为null</param>
    Task<int> GetCountAsync(Guid? albumId, Guid? memberId);

    /// <summary>
    /// 插入单张照片
    /// </summary>
    /// <param name="photo">待插入的照片实体</param>
    Task<int> InsertAsync(Photo photo);

    /// <summary>
    /// 批量插入照片
    /// </summary>
    /// <param name="photos">待插入的照片实体列表</param>
    Task<int> InsertRangeAsync(List<Photo> photos);

    /// <summary>
    /// 更新照片
    /// </summary>
    /// <param name="photo">待更新的照片实体</param>
    Task<int> UpdateAsync(Photo photo);

    /// <summary>
    /// 根据ID删除照片
    /// </summary>
    /// <param name="id">照片ID</param>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 根据相册ID删除所有照片
    /// </summary>
    /// <param name="albumId">相册ID</param>
    Task<int> DeleteByAlbumIdAsync(Guid albumId);

    /// <summary>
    /// 判断指定ID的照片是否存在
    /// </summary>
    /// <param name="id">照片ID</param>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// 获取相册中最早上传的照片（用作封面）
    /// </summary>
    /// <param name="albumId">相册ID</param>
    Task<Photo?> GetFirstPhotoByAlbumIdAsync(Guid albumId);
}
