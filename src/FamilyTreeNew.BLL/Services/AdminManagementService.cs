using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Models.Helpers;

namespace FamilyTreeNew.BLL.Services;

public interface IAdminManagementService
{
    Task<PagedResult<AdminDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);
    Task<AdminDto?> GetByIdAsync(Guid id);
    Task<AdminDto> CreateAsync(CreateAdminDto dto);
    Task<AdminDto?> UpdateAsync(Guid id, UpdateAdminDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class AdminManagementService : IAdminManagementService
{
    private readonly IAdminRepository _adminRepository;

    public AdminManagementService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<PagedResult<AdminDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null)
    {
        var trimmedKeyword = keyword?.Trim();
        var (items, totalCount) = await _adminRepository.GetPagedAsync(
            pageIndex,
            pageSize,
            string.IsNullOrWhiteSpace(trimmedKeyword)
                ? null
                : admin => admin.Username.Contains(trimmedKeyword) || (admin.RealName != null && admin.RealName.Contains(trimmedKeyword)));

        return new PagedResult<AdminDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<AdminDto?> GetByIdAsync(Guid id)
    {
        var admin = await _adminRepository.GetByIdAsync(id);
        return admin == null ? null : MapToDto(admin);
    }

    public async Task<AdminDto> CreateAsync(CreateAdminDto dto)
    {
        if (await _adminRepository.ExistsByUsernameAsync(dto.Username))
        {
            throw new InvalidOperationException("用户名已存在");
        }

        var validationResult = PasswordValidator.Validate(dto.Password, minLength: 8, requireUppercase: true,
            requireLowercase: true, requireDigit: true, requireSpecialChar: true);

        if (!validationResult.IsValid)
        {
            throw new ArgumentException(string.Join("；", validationResult.Errors));
        }

        var hashedPassword = PasswordHelper.HashPassword(dto.Password, out var salt);
        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            Username = dto.Username.Trim(),
            Password = hashedPassword,
            PasswordSalt = salt,
            RealName = dto.RealName?.Trim(),
            Email = dto.Email?.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsEnabled = true
        };

        await _adminRepository.InsertAsync(admin);
        return MapToDto(admin);
    }

    public async Task<AdminDto?> UpdateAsync(Guid id, UpdateAdminDto dto)
    {
        var admin = await _adminRepository.GetByIdAsync(id);
        if (admin == null)
        {
            return null;
        }

        var existingAdmin = await _adminRepository.GetByUsernameAsync(dto.Username.Trim());
        if (existingAdmin != null && existingAdmin.Id != id)
        {
            throw new InvalidOperationException("用户名已存在");
        }

        admin.Username = dto.Username.Trim();
        admin.RealName = dto.RealName?.Trim();
        admin.Email = dto.Email?.Trim();
        admin.IsEnabled = dto.IsEnabled;

        await _adminRepository.UpdateAsync(admin);
        return MapToDto(admin);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _adminRepository.ExistsAsync(id))
        {
            return false;
        }

        var admins = await _adminRepository.GetAllAsync();
        if (admins.Count <= 1)
        {
            throw new InvalidOperationException("至少需要保留一个系统管理员账号");
        }

        await _adminRepository.DeleteAsync(id);
        return true;
    }

    private static AdminDto MapToDto(Admin admin)
    {
        return new AdminDto
        {
            Id = admin.Id,
            Username = admin.Username,
            RealName = admin.RealName,
            Email = admin.Email,
            CreatedAt = admin.CreatedAt,
            LastLoginAt = admin.LastLoginAt,
            IsEnabled = admin.IsEnabled
        };
    }
}
