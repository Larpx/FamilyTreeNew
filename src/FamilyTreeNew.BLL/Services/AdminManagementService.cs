using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Models.Helpers;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

public class AdminManagementService : IAdminManagementService
{
    private readonly IAdminRepository _adminRepository;
    private readonly PasswordValidator _passwordValidator;
    private readonly ILogger<AdminManagementService> _logger;

    public AdminManagementService(IAdminRepository adminRepository, PasswordValidator passwordValidator, ILogger<AdminManagementService> logger)
    {
        _adminRepository = adminRepository;
        _passwordValidator = passwordValidator;
        _logger = logger;
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

        var validationResult = _passwordValidator.Validate(dto.Password);

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
        _logger.LogInformation("创建管理员，ID: {AdminId}，用户名: {Username}", admin.Id, admin.Username);
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
        _logger.LogInformation("更新管理员，ID: {AdminId}", id);
        return MapToDto(admin);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _adminRepository.ExistsAsync(id))
        {
            return false;
        }

        var totalCount = await _adminRepository.GetCountAsync();
        if (totalCount <= 1)
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
