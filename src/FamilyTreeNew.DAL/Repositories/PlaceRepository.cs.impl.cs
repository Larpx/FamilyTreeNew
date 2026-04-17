using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 地点仓储实现。
/// 提供按省、市和启用状态筛选地点的数据库查询能力。
/// </summary>
public class PlaceRepository : BaseRepositoryGuid<Place>, IPlaceRepository
{
    public PlaceRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<Place>> GetByProvinceAsync(string province)
    {
        return await Db.Queryable<Place>().Where(p => p.Province == province).ToListAsync();
    }

    public async Task<List<Place>> GetByCityAsync(string city)
    {
        return await Db.Queryable<Place>().Where(p => p.City == city).ToListAsync();
    }

    public async Task<List<Place>> GetEnabledPlacesAsync()
    {
        return await Db.Queryable<Place>().Where(p => p.IsEnabled).ToListAsync();
    }
}