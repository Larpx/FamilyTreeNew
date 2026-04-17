using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 地点服务。
/// 负责把地点仓储的数据转换为 DTO，并提供地点的查询、创建和更新能力。
/// </summary>
public class PlaceService : IPlaceService
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IEventRepository _eventRepository;

    public PlaceService(IPlaceRepository placeRepository, IEventRepository eventRepository)
    {
        _placeRepository = placeRepository;
        _eventRepository = eventRepository;
    }

    /// <summary>
    /// 获取全部地点。
    /// </summary>
    public async Task<List<PlaceResponseDto>> GetAllAsync()
    {
        var entities = await _placeRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 获取启用中的地点。
    /// </summary>
    public async Task<List<PlaceResponseDto>> GetEnabledPlacesAsync()
    {
        var entities = await _placeRepository.GetEnabledPlacesAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 按省份获取地点。
    /// </summary>
    public async Task<List<PlaceResponseDto>> GetByProvinceAsync(string province)
    {
        var entities = await _placeRepository.GetByProvinceAsync(province);
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 按城市获取地点。
    /// </summary>
    public async Task<List<PlaceResponseDto>> GetByCityAsync(string city)
    {
        var entities = await _placeRepository.GetByCityAsync(city);
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 根据 ID 获取地点。
    /// </summary>
    public async Task<PlaceResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _placeRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 创建地点。
    /// </summary>
    public async Task<PlaceResponseDto> CreateAsync(PlaceCreateRequestDto dto)
    {
        var entity = new Place
        {
            Name = dto.Name,
            Address = dto.Address,
            Province = dto.Province,
            City = dto.City,
            District = dto.District,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Description = dto.Description,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _placeRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 更新地点。
    /// </summary>
    public async Task<PlaceResponseDto?> UpdateAsync(Guid id, PlaceUpdateRequestDto dto)
    {
        var entity = await _placeRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Address = dto.Address;
        entity.Province = dto.Province;
        entity.City = dto.City;
        entity.District = dto.District;
        entity.Latitude = dto.Latitude;
        entity.Longitude = dto.Longitude;
        entity.Description = dto.Description;
        entity.IsEnabled = dto.IsEnabled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _placeRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 删除地点。
    /// 如果地点已被事件引用，则拒绝删除。
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _placeRepository.ExistsAsync(id)) return false;

        var relatedEvents = await _eventRepository.GetByPlaceIdAsync(id);
        if (relatedEvents.Count > 0)
        {
            throw new InvalidOperationException("该地点下存在关联事件，无法删除");
        }

        await _placeRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 将地点实体转换为 DTO。
    /// </summary>
    private static PlaceResponseDto MapToDto(Place entity)
    {
        return new PlaceResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Address = entity.Address,
            Province = entity.Province,
            City = entity.City,
            District = entity.District,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Description = entity.Description,
            IsEnabled = entity.IsEnabled,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}