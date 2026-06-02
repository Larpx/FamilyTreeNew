using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

public class PlaceService : IPlaceService
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<PlaceService> _logger;

    public PlaceService(IPlaceRepository placeRepository, IEventRepository eventRepository, ILogger<PlaceService> logger)
    {
        _placeRepository = placeRepository;
        _eventRepository = eventRepository;
        _logger = logger;
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
            CreatedAt = DateTime.UtcNow
        };

        await _placeRepository.InsertAsync(entity);
        _logger.LogInformation("创建地点，ID: {PlaceId}，名称: {Name}", entity.Id, dto.Name);
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
        entity.UpdatedAt = DateTime.UtcNow;

        await _placeRepository.UpdateAsync(entity);
        _logger.LogInformation("更新地点，ID: {PlaceId}", id);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _placeRepository.ExistsAsync(id)) return false;

        var relatedEvents = await _eventRepository.GetByPlaceIdAsync(id);
        if (relatedEvents.Count > 0)
        {
            throw new InvalidOperationException("该地点下存在关联事件，无法删除");
        }

        await _placeRepository.DeleteAsync(id);
        _logger.LogInformation("删除地点，ID: {PlaceId}", id);
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