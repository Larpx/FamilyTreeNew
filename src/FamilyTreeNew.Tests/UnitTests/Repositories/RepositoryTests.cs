using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.Entities;
using FluentAssertions;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Repositories;

/// <summary>
/// 管理员仓储接口与实现测试。
/// 用来确认管理员仓储提供了需要的查询、保存和删除方法。
/// </summary>
public class AdminRepositoryTests
{
    [Fact]
    public void IAdminRepository_ShouldHaveRequiredMethods()
    {
        typeof(IAdminRepository).Should().HaveMethod("GetByUsernameAsync", new[] { typeof(string) });
        typeof(IAdminRepository).Should().HaveMethod("GetByIdAsync", new[] { typeof(Guid) });
        typeof(IAdminRepository).Should().HaveMethod("InsertAsync", new[] { typeof(Admin) });
        typeof(IAdminRepository).Should().HaveMethod("UpdateAsync", new[] { typeof(Admin) });
        typeof(IAdminRepository).Should().HaveMethod("UpdateLastLoginTimeAsync", new[] { typeof(Guid) });
        typeof(IAdminRepository).Should().HaveMethod("ExistsByUsernameAsync", new[] { typeof(string) });
        typeof(IAdminRepository).Should().HaveMethod("ExistsAsync", new[] { typeof(Guid) });
        typeof(IAdminRepository).Should().HaveMethod("DeleteAsync", new[] { typeof(Guid) });
    }

    [Fact]
    public void AdminRepository_ShouldImplementIBaseRepositoryGuid()
    {
        typeof(AdminRepository).Should().Implement<IAdminRepository>();
    }
}

/// <summary>
/// 操作日志仓储接口与实现测试。
/// 用来确认操作日志查询与统计相关的方法是否齐全。
/// </summary>
public class OperationLogRepositoryTests
{
    [Fact]
    public void IOperationLogRepository_ShouldHaveRequiredMethods()
    {
        typeof(IOperationLogRepository).Should().HaveMethod("GetListAsync", new[] { typeof(int), typeof(int) });
        typeof(IOperationLogRepository).Should().HaveMethod("GetByAdminIdAsync", new[] { typeof(Guid), typeof(int), typeof(int) });
        typeof(IOperationLogRepository).Should().HaveMethod("GetCountAsync", Type.EmptyTypes);
        typeof(IOperationLogRepository).Should().HaveMethod("GetCountByAdminIdAsync", new[] { typeof(Guid) });
    }

    [Fact]
    public void OperationLogRepository_ShouldImplementIBaseRepositoryGuid()
    {
        typeof(OperationLogRepository).Should().Implement<IOperationLogRepository>();
    }
}

/// <summary>
/// 相册仓储接口测试。
/// 用来确认相册分页、查询和关联检查所需的方法都已定义。
/// </summary>
public class AlbumRepositoryTests
{
    [Fact]
    public void IAlbumRepository_ShouldHaveRequiredMethods()
    {
        typeof(IAlbumRepository).Should().HaveMethod("GetByIdAsync", new[] { typeof(Guid) });
        typeof(IAlbumRepository).Should().HaveMethod("GetByIdWithPhotosAsync", new[] { typeof(Guid) });
        typeof(IAlbumRepository).Should().HaveMethod("GetByFamilyTreeIdAsync", new[] { typeof(Guid) });
        typeof(IAlbumRepository).Should().HaveMethod("GetPagedAsync", new[] { typeof(int), typeof(int), typeof(Guid?), typeof(string) });
        typeof(IAlbumRepository).Should().HaveMethod("GetCountAsync", new[] { typeof(Guid?), typeof(string) });
        typeof(IAlbumRepository).Should().HaveMethod("InsertAsync", new[] { typeof(Album) });
        typeof(IAlbumRepository).Should().HaveMethod("UpdateAsync", new[] { typeof(Album) });
        typeof(IAlbumRepository).Should().HaveMethod("DeleteAsync", new[] { typeof(Guid) });
        typeof(IAlbumRepository).Should().HaveMethod("ExistsAsync", new[] { typeof(Guid) });
        typeof(IAlbumRepository).Should().HaveMethod("ExistsInFamilyTreeAsync", new[] { typeof(Guid), typeof(Guid) });
    }
}

/// <summary>
/// 照片仓储接口测试。
/// 用来确认照片查询、分页和批量操作相关的方法都已定义。
/// </summary>
public class PhotoRepositoryTests
{
    [Fact]
    public void IPhotoRepository_ShouldHaveRequiredMethods()
    {
        typeof(IPhotoRepository).Should().HaveMethod("GetByIdAsync", new[] { typeof(Guid) });
        typeof(IPhotoRepository).Should().HaveMethod("GetByAlbumIdAsync", new[] { typeof(Guid) });
        typeof(IPhotoRepository).Should().HaveMethod("GetByMemberIdAsync", new[] { typeof(Guid) });
        typeof(IPhotoRepository).Should().HaveMethod("GetPagedAsync", new[] { typeof(int), typeof(int), typeof(Guid?), typeof(Guid?) });
        typeof(IPhotoRepository).Should().HaveMethod("GetCountAsync", new[] { typeof(Guid?), typeof(Guid?) });
        typeof(IPhotoRepository).Should().HaveMethod("InsertAsync", new[] { typeof(Photo) });
        typeof(IPhotoRepository).Should().HaveMethod("InsertRangeAsync", new[] { typeof(List<Photo>) });
        typeof(IPhotoRepository).Should().HaveMethod("UpdateAsync", new[] { typeof(Photo) });
        typeof(IPhotoRepository).Should().HaveMethod("DeleteAsync", new[] { typeof(Guid) });
        typeof(IPhotoRepository).Should().HaveMethod("DeleteByAlbumIdAsync", new[] { typeof(Guid) });
        typeof(IPhotoRepository).Should().HaveMethod("ExistsAsync", new[] { typeof(Guid) });
        typeof(IPhotoRepository).Should().HaveMethod("GetFirstPhotoByAlbumIdAsync", new[] { typeof(Guid) });
    }
}

/// <summary>
/// 家谱仓储测试。
/// 用来确认家谱仓储实现了带成员数量统计的分页查询能力。
/// </summary>
public class FamilyTreeRepositoryTests
{
    [Fact]
    public void FamilyTreeRepository_Interface_ShouldHaveRequiredMethods()
    {
        typeof(IFamilyTreeRepository).Should().HaveMethod("GetPagedWithMemberCountAsync", new[] { typeof(int), typeof(int), typeof(string), typeof(bool?) });
        typeof(IFamilyTreeRepository).Should().HaveMethod("GetByIdWithMemberCountAsync", new[] { typeof(Guid) });
        typeof(IFamilyTreeRepository).Should().HaveMethod("GetMemberCountAsync", new[] { typeof(Guid) });
        typeof(IFamilyTreeRepository).Should().HaveMethod("GetMemberCountsAsync", new[] { typeof(List<Guid>) });
    }

    [Fact]
    public void FamilyTreeRepository_ShouldImplementIBaseRepositoryGuid()
    {
        typeof(FamilyTreeRepository).Should().Implement<IBaseRepositoryGuid<FamilyTree>>();
    }
}

/// <summary>
/// 家谱成员仓储测试。
/// 用来确认成员仓储实现了父子关系、分页查询和批量插入等方法。
/// </summary>
public class FamilyMemberRepositoryTests
{
    [Fact]
    public void FamilyMemberRepository_Interface_ShouldHaveRequiredMethods()
    {
        typeof(IFamilyMemberRepository).Should().HaveMethod("GetByIdAsync", new[] { typeof(Guid) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("GetByIdWithParentAsync", new[] { typeof(Guid) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("ExistsAsync", new[] { typeof(Guid) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("ExistsInFamilyTreeAsync", new[] { typeof(Guid), typeof(Guid) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("InsertAsync", new[] { typeof(FamilyMember) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("UpdateAsync", new[] { typeof(FamilyMember) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("DeleteAsync", new[] { typeof(Guid) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("GetPagedByFamilyTreeAsync", new[] { typeof(Guid), typeof(int), typeof(int), typeof(string), typeof(int?), typeof(Guid?) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("GetByFamilyTreeIdAsync", new[] { typeof(Guid) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("HasChildrenAsync", new[] { typeof(Guid) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("BulkInsertAsync", new[] { typeof(List<FamilyMember>) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("GetGenerationByParentIdAsync", new[] { typeof(Guid?) });
        typeof(IFamilyMemberRepository).Should().HaveMethod("GetParentNamesAsync", new[] { typeof(List<Guid>) });
    }
}

/// <summary>
/// 通用 GUID 仓储接口测试。
/// 用来确认基类仓储接口包含常用的 CRUD 和分页能力。
/// </summary>
public class BaseRepositoryGuidTests
{
    [Fact]
    public void IBaseRepositoryGuid_ShouldHaveRequiredMethods()
    {
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveMethod("GetAllAsync", Type.EmptyTypes);
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveMethod("GetByIdAsync", new[] { typeof(Guid) });
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveMethod("GetPagedAsync", new[] { typeof(int), typeof(int), typeof(System.Linq.Expressions.Expression<Func<FamilyTree, bool>>) });
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveMethod("InsertAsync", new[] { typeof(FamilyTree) });
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveMethod("UpdateAsync", new[] { typeof(FamilyTree) });
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveMethod("DeleteAsync", new[] { typeof(Guid) });
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveMethod("ExistsAsync", new[] { typeof(Guid) });
    }

    [Fact]
    public void IBaseRepositoryGuid_ShouldHaveDbProperty()
    {
        typeof(IBaseRepositoryGuid<FamilyTree>).Should().HaveProperty<SqlSugar.ISqlSugarClient>("Db");
    }
}
