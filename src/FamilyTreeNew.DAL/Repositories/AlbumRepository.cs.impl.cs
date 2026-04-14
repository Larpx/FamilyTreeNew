using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;
using SqlSugar;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 相册仓储实现，继承自BaseRepositoryGuid
/// </summary>
public class AlbumRepository : BaseRepositoryGuid<Album>, IAlbumRepository
{
    public AlbumRepository(SqlSugarContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<Album?> GetByIdWithPhotosAsync(Guid id)
    {
        return await Db.Queryable<Album>()
            .Where(a => a.Id == id)
            .Includes(a => a.Photos)
            .FirstAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Album>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Queryable<Album>()
            .Where(a => a.FamilyTreeId == familyTreeId)
            .OrderBy(a => a.CreatedAt, OrderByType.Desc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Album>> GetPagedAsync(int pageIndex, int pageSize, Guid? familyTreeId, string? keyword)
    {
        var query = Db.Queryable<Album>()
            .WhereIF(familyTreeId.HasValue, a => a.FamilyTreeId == familyTreeId!.Value)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), a => a.Name.Contains(keyword!) || (a.Description != null && a.Description.Contains(keyword ?? "")));

        return await query
            .OrderBy(a => a.CreatedAt, OrderByType.Desc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> GetCountAsync(Guid? familyTreeId, string? keyword)
    {
        return await Db.Queryable<Album>()
            .WhereIF(familyTreeId.HasValue, a => a.FamilyTreeId == familyTreeId!.Value)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), a => a.Name.Contains(keyword!) || (a.Description != null && a.Description.Contains(keyword ?? "")))
            .CountAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsInFamilyTreeAsync(Guid id, Guid familyTreeId)
    {
        return await Db.Queryable<Album>()
            .Where(a => a.Id == id && a.FamilyTreeId == familyTreeId)
            .AnyAsync();
    }
}
