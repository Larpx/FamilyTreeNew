using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class PlaceService : IPlaceService
{
    private readonly IPlaceRepository _placeRepository;

    public PlaceService(IPlaceRepository placeRepository)
    {
        _placeRepository = placeRepository;
    }

    public async Task<List<PlaceResponseDto>> GetAllAsync()
    {
        var entities = await _placeRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<PlaceResponseDto>> GetEnabledPlacesAsync()
    {
        var entities = await _placeRepository.GetEnabledPlacesAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<PlaceResponseDto>> GetByProvinceAsync(string province)
    {
        var entities = await _placeRepository.GetByProvinceAsync(province);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<PlaceResponseDto>> GetByCityAsync(string city)
    {
        var entities = await _placeRepository.GetByCityAsync(city);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<PlaceResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _placeRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

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
            CreatedAt = DateTime.Now
        };

        await _placeRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

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
        entity.UpdatedAt = DateTime.Now;

        await _placeRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _placeRepository.ExistsAsync(id)) return false;
        await _placeRepository.DeleteAsync(id);
        return true;
    }

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