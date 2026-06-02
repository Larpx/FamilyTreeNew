using FamilyTreeNew.BLL.Configuration;
using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeNew.BLL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        if (configuration != null)
        {
            var jwtSettings = configuration.GetJwtSettings();
            services.AddSingleton(jwtSettings);

            var maxImageFileSize = configuration.GetValue<long?>("FileUpload:MaxImageFileSizeBytes");
            if (maxImageFileSize.HasValue)
            {
                FileHelper.Configure(maxImageFileSize.Value);
            }
        }

        services.AddScoped<IFamilyService, FamilyService>();
        services.AddScoped<IFamilyTreeService, FamilyTreeService>();
        services.AddScoped<IFamilyMemberService, FamilyMemberService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddScoped<IVerificationQuestionService, VerificationQuestionService>();
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IAdminManagementService, AdminManagementService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOperationLogService, OperationLogService>();
        services.AddScoped<ISystemSettingsService, SystemSettingsService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddScoped<IBackupService, BackupService>();
        services.AddScoped<IJwtHelper, JwtHelper>();
        services.AddScoped<IAlbumService, AlbumService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ISpousalRelationService, SpousalRelationService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IEventTypeService, EventTypeService>();
        services.AddScoped<IPlaceService, PlaceService>();
        services.AddScoped<ISourceService, SourceService>();
        services.AddScoped<ISourceCitationService, SourceCitationService>();
        services.AddScoped<IGedcomService, GedcomService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<PasswordValidator>();
        return services;
    }
}
