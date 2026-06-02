using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace FamilyTreeNew.BLL.Services;

public class SystemSettingsService : ISystemSettingsService
{
    private readonly SqlSugarContext _context;
    private readonly ILogger<SystemSettingsService> _logger;
    private readonly IConfiguration _configuration;

    public SystemSettingsService(
        SqlSugarContext context,
        ILogger<SystemSettingsService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
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
                settings.CreatedAt = DateTime.UtcNow;
                await _context.Db.Insertable(settings).ExecuteCommandAsync();
            }
            else
            {
                MapFromDto(settings, dto);
                settings.UpdatedAt = DateTime.UtcNow;
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
        var section = _configuration.GetSection("SystemSettings:Defaults");
        var settings = new SystemSettings
        {
            SiteName = section.GetValue("SiteName", "家族族谱"),
            SiteDescription = section.GetValue("SiteDescription", "记录家族历史，传承家族文化"),
            ThemeColor = section.GetValue("ThemeColor", "#1890ff"),
            ShowStatistics = section.GetValue("ShowStatistics", true),
            AllowGuestBrowse = section.GetValue("AllowGuestBrowse", false),
            RequireVerification = section.GetValue("RequireVerification", true),
            MaxLoginAttempts = section.GetValue("MaxLoginAttempts", 5),
            LockoutDuration = section.GetValue("LockoutDuration", 30),
            SessionTimeout = section.GetValue("SessionTimeout", 120),
            EnableOperationLog = section.GetValue("EnableOperationLog", true),
            LogRetentionDays = section.GetValue("LogRetentionDays", 90),
            CreatedAt = DateTime.UtcNow
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
