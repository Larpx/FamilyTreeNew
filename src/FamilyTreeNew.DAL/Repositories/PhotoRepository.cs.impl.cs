using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;
using SqlSugar;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 照片仓储实现，继承自BaseRepositoryGuid
/// </summary>
public class PhotoRepository : BaseRepositoryGuid<Photo>, IPhotoRepository
{
    public PhotoRepository(SqlSugarContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<List<Photo>> GetByAlbumIdAsync(Guid albumId)
    {
        return await Db.Queryable<Photo>()
            .Where(p => p.AlbumId == albumId)
            .OrderBy(p => p.UploadedAt, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Photo>> GetByMemberIdAsync(Guid memberId)
    {
        return await Db.Queryable<Photo>()
            .Where(p => p.MemberId == memberId)
            .OrderBy(p => p.UploadedAt, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Photo>> GetPagedAsync(int pageIndex, int pageSize, Guid? albumId, Guid? memberId)
    {
        var query = Db.Queryable<Photo>()
            .WhereIF(albumId.HasValue, p => p.AlbumId == albumId!.Value)
            .WhereIF(memberId.HasValue, p => p.MemberId == memberId!.Value);

        return await query
            .OrderBy(p => p.UploadedAt, OrderByType.Desc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> GetCountAsync(Guid? albumId, Guid? memberId)
    {
        return await Db.Queryable<Photo>()
            .WhereIF(albumId.HasValue, p => p.AlbumId == albumId!.Value)
            .WhereIF(memberId.HasValue, p => p.MemberId == memberId!.Value)
            .CountAsync();
    }

    /// <inheritdoc/>
    public async Task<int> InsertRangeAsync(List<Photo> photos)
    {
        return await Db.Insertable(photos).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> DeleteByAlbumIdAsync(Guid albumId)
    {
        return await Db.Deleteable<Photo>().Where(p => p.AlbumId == albumId).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<Photo?> GetFirstPhotoByAlbumIdAsync(Guid albumId)
    {
        return await Db.Queryable<Photo>()
            .Where(p => p.AlbumId == albumId)
            .OrderBy(p => p.UploadedAt, OrderByType.Asc)
            .FirstAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Photo>> GetByAlbumIdsAsync(List<Guid> albumIds)
    {
        if (albumIds == null || albumIds.Count == 0)
        {
            return new List<Photo>();
        }

        return await Db.Queryable<Photo>()
            .Where(p => albumIds.Contains(p.AlbumId))
            .OrderBy(p => p.UploadedAt, OrderByType.Desc)
            .ToListAsync();
    }
}
