using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IPlaceRepository : IBaseRepositoryGuid<Place>
{
    Task<List<Place>> GetByProvinceAsync(string province);
    
    Task<List<Place>> GetByCityAsync(string city);
    
    Task<List<Place>> GetEnabledPlacesAsync();
}