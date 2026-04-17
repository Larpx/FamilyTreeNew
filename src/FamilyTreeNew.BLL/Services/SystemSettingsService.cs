using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

public interface ISystemSettingsService
{
    Task<SystemSettingsDto?> GetSettingsAsync();
    Task<SystemSettingsDto?> UpdateSettingsAsync(UpdateSystemSettingsDto dto);
}

public class SystemSettingsService : ISystemSettingsService
{
    private readonly SqlSugarContext _context;
    private readonly ILogger<SystemSettingsService> _logger;

    public SystemSettingsService(
        SqlSugarContext context,
        ILogger<SystemSettingsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SystemSettingsDto?> GetSettingsAsync()
    {
        try
        {
            var settings = await _context.Db.Queryable<SystemSettings>()
                .FirstAsync();

            if (settings == null)
            {
                settings = await CreateDefaultSettingsAsync();
            }

            return MapToDto(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system settings");
            return null;
        }
    }

    public async Task<SystemSettingsDto?> UpdateSettingsAsync(UpdateSystemSettingsDto dto)
    {
        try
        {
            var settings = await _context.Db.Queryable<SystemSettings>()
                .FirstAsync();

            if (settings == null)
            {
                settings = new SystemSettings();
                MapFromDto(settings, dto);
                settings.CreatedAt = DateTime.Now;
                await _context.Db.Insertable(settings).ExecuteCommandAsync();
            }
            else
            {
                MapFromDto(settings, dto);
                settings.UpdatedAt = DateTime.Now;
                await _context.Db.Updateable(settings).ExecuteCommandAsync();
            }

            _logger.LogInformation("System settings updated successfully");
            return MapToDto(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating system settings");
            return null;
        }
    }

    private async Task<SystemSettings> CreateDefaultSettingsAsync()
    {
        var settings = new SystemSettings
        {
            SiteName = "家族族谱",
            SiteDescription = "记录家族历史，传承家族文化",
            ThemeColor = "#1890ff",
            ShowStatistics = true,
            AllowGuestBrowse = false,
            RequireVerification = true,
            MaxLoginAttempts = 5,
            LockoutDuration = 30,
            SessionTimeout = 120,
            EnableOperationLog = true,
            LogRetentionDays = 90,
            CreatedAt = DateTime.Now
        };

        await _context.Db.Insertable(settings).ExecuteCommandAsync();
        _logger.LogInformation("Created default system settings");
        return settings;
    }

    private static SystemSettingsDto MapToDto(SystemSettings settings)
    {
        return new SystemSettingsDto
        {
            Id = settings.Id,
            SiteName = settings.SiteName,
            SiteDescription = settings.SiteDescription,
            LogoUrl = settings.LogoUrl,
            FaviconUrl = settings.FaviconUrl,
            ThemeColor = settings.ThemeColor,
            ShowStatistics = settings.ShowStatistics,
            AllowGuestBrowse = settings.AllowGuestBrowse,
            RequireVerification = settings.RequireVerification,
            MaxLoginAttempts = settings.MaxLoginAttempts,
            LockoutDuration = settings.LockoutDuration,
            SessionTimeout = settings.SessionTimeout,
            EnableOperationLog = settings.EnableOperationLog,
            LogRetentionDays = settings.LogRetentionDays,
            ContactEmail = settings.ContactEmail,
            ContactPhone = settings.ContactPhone,
            ContactAddress = settings.ContactAddress,
            FooterText = settings.FooterText
        };
    }

    private static void MapFromDto(SystemSettings settings, UpdateSystemSettingsDto dto)
    {
        settings.SiteName = dto.SiteName;
        settings.SiteDescription = dto.SiteDescription;
        settings.LogoUrl = dto.LogoUrl;
        settings.FaviconUrl = dto.FaviconUrl;
        settings.ThemeColor = dto.ThemeColor;
        settings.ShowStatistics = dto.ShowStatistics;
        settings.AllowGuestBrowse = dto.AllowGuestBrowse;
        settings.RequireVerification = dto.RequireVerification;
        settings.MaxLoginAttempts = dto.MaxLoginAttempts;
        settings.LockoutDuration = dto.LockoutDuration;
        settings.SessionTimeout = dto.SessionTimeout;
        settings.EnableOperationLog = dto.EnableOperationLog;
        settings.LogRetentionDays = dto.LogRetentionDays;
        settings.ContactEmail = dto.ContactEmail;
        settings.ContactPhone = dto.ContactPhone;
        settings.ContactAddress = dto.ContactAddress;
        settings.FooterText = dto.FooterText;
    }
}
