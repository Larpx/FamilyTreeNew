using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.DAL.Context;

namespace FamilyTreeNew.DAL.Repositories;

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