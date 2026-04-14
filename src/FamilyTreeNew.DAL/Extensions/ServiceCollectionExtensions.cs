using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace FamilyTreeNew.DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalServices(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<SqlSugarContext>(sp => new SqlSugarContext(connectionString));
        services.AddScoped<ISqlSugarClient>(sp => sp.GetRequiredService<SqlSugarContext>().Db);
        services.AddScoped<IFamilyRepository, FamilyRepository>();
        services.AddScoped<IFamilyTreeRepository, FamilyTreeRepository>();
        services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
        services.AddScoped<IVerificationQuestionRepository, VerificationQuestionRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IOperationLogRepository, OperationLogRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<ISpousalRelationRepository, SpousalRelationRepository>();
        services.AddScoped<DatabaseInitializer>();
        
        return services;
    }
}
