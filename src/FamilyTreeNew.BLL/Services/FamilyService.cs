using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

public class FamilyService : IFamilyService
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;
    private readonly ILogger<FamilyService> _logger;

    public FamilyService(IFamilyRepository familyRepository, IFamilyMemberRepository familyMemberRepository, ILogger<FamilyService> logger)
    {
        _familyRepository = familyRepository;
        _familyMemberRepository = familyMemberRepository;
        _logger = logger;
    }

    public async Task<List<FamilyResponseDto>> GetAllFamiliesAsync()
    {
        var families = await _familyRepository.GetFamiliesWithMemberCountAsync();
        return families.Select(MapToDto).ToList();
    }

    public async Task<FamilyResponseDto?> GetFamilyByIdAsync(int id)
    {
        var family = await _familyRepository.GetByIdAsync(id);
        return family != null ? await MapToDtoAsync(family) : null;
    }

    public async Task<FamilyResponseDto> CreateFamilyAsync(FamilyCreateRequestDto dto)
    {
        var entity = new Family
        {
            FamilyTreeId = dto.FamilyTreeId,
            FamilyName = dto.FamilyName,
            HeadMemberId = dto.HeadMemberId,
            Address = dto.Address,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _familyRepository.InsertAsync(entity);
        _logger.LogInformation("创建家族，ID: {FamilyId}，名称: {FamilyName}", entity.Id, dto.FamilyName);
        return MapToDto(entity);
    }

    public async Task<FamilyResponseDto?> UpdateFamilyAsync(int id, FamilyUpdateRequestDto dto)
    {
        var entity = await _familyRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.FamilyName = dto.FamilyName;
        entity.HeadMemberId = dto.HeadMemberId;
        entity.Address = dto.Address;
        entity.Description = dto.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        await _familyRepository.UpdateAsync(entity);
        _logger.LogInformation("更新家族，ID: {FamilyId}", id);
        return await MapToDtoAsync(entity);
    }

    public async Task<bool> DeleteFamilyAsync(int id)
    {
        var entity = await _familyRepository.GetByIdAsync(id);
        if (entity == null) return false;
        await _familyRepository.DeleteAsync(id);
        _logger.LogInformation("删除家族，ID: {FamilyId}", id);
        return true;
    }

    private static FamilyResponseDto MapToDto(Family entity)
    {
        return new FamilyResponseDto
        {
            Id = entity.Id,
            FamilyTreeId = entity.FamilyTreeId ?? Guid.Empty,
            FamilyName = entity.FamilyName,
            HeadMemberId = entity.HeadMemberId,
            Address = entity.Address,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private async Task<FamilyResponseDto> MapToDtoAsync(Family entity)
    {
        var dto = MapToDto(entity);
        if (entity.HeadMemberId.HasValue)
        {
            var member = await _familyMemberRepository.GetByIdAsync(entity.HeadMemberId.Value);
            dto.HeadMemberName = member != null ? $"{member.Surname}{member.FirstName}" : null;
        }
        return dto;
    }
}
