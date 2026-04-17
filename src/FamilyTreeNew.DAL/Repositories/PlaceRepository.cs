using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 地点仓储接口。
/// 负责按省市查询地点并获取启用状态的地点列表。
/// </summary>
public interface IPlaceRepository : IBaseRepositoryGuid<Place>
{
    Task<List<Place>> GetByProvinceAsync(string province);

    Task<List<Place>> GetByCityAsync(string city);

    Task<List<Place>> GetEnabledPlacesAsync();
}