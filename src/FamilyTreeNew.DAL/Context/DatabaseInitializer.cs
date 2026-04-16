using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Models.Helpers;
using SqlSugar;

namespace FamilyTreeNew.DAL.Context;

public class DatabaseInitializer
{
    private readonly ISqlSugarClient _db;

    public DatabaseInitializer(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task InitializeAsync()
    {
        await CreateTablesAsync();
        await CreateIndexesAsync();
        await SeedDataAsync();
    }

    private Task CreateTablesAsync()
    {
        _db.CodeFirst.InitTables(
            typeof(FamilyTree),
            typeof(FamilyMember),
            typeof(Admin),
            typeof(VerificationQuestion),
            typeof(Album),
            typeof(Photo),
            typeof(OperationLog),
            typeof(SystemSettings),
            typeof(Family),
            typeof(Role),
            typeof(Permission),
            typeof(UserRole),
            typeof(RolePermission),
            typeof(Menu),
            typeof(SpousalRelation),
            typeof(EventType),
            typeof(Place),
            typeof(Event),
            typeof(Source),
            typeof(SourceCitation)
        );

        return Task.CompletedTask;
    }

    private async Task CreateIndexesAsync()
    {
        var indexes = new List<(string IndexName, string TableName, string ColumnName, bool IsUnique)>
        {
            ("idx_family_members_family_tree_id", "FamilyMembers", "FamilyTreeId", false),
            ("idx_family_members_parent_id", "FamilyMembers", "ParentId", false),
            ("idx_family_members_generation", "FamilyMembers", "Generation", false),
            ("idx_verification_questions_family_tree_id", "VerificationQuestions", "FamilyTreeId", false),
            ("idx_albums_family_tree_id", "Albums", "FamilyTreeId", false),
            ("idx_photos_album_id", "Photos", "AlbumId", false),
            ("idx_photos_member_id", "Photos", "MemberId", false),
            ("idx_operation_logs_admin_id", "OperationLogs", "AdminId", false),
            ("idx_operation_logs_operation_time", "OperationLogs", "OperationTime", false),
            ("idx_admins_username", "Admins", "Username", true)
        };

        var compositeIndexes = new List<(string IndexName, string TableName, string[] Columns)>
        {
            ("idx_family_members_tree_parent", "FamilyMembers", new[] { "FamilyTreeId", "ParentId" }),
            ("idx_family_members_tree_generation", "FamilyMembers", new[] { "FamilyTreeId", "Generation" }),
            ("idx_family_members_created_at", "FamilyMembers", new[] { "FamilyTreeId", "CreatedAt" }),
            ("idx_albums_created_at", "Albums", new[] { "FamilyTreeId", "CreatedAt" }),
            ("idx_photos_uploaded_at", "Photos", new[] { "AlbumId", "UploadedAt" })
        };

        foreach (var (indexName, tableName, columnName, isUnique) in indexes)
        {
            try
            {
                if (!_db.DbMaintenance.IsAnyIndex(indexName))
                {
                    _db.DbMaintenance.CreateIndex(tableName, new[] { columnName }, indexName, isUnique);
                }
            }
            catch (Exception)
            {
            }
        }

        foreach (var (indexName, tableName, columns) in compositeIndexes)
        {
            try
            {
                if (!_db.DbMaintenance.IsAnyIndex(indexName))
                {
                    _db.DbMaintenance.CreateIndex(tableName, columns, indexName);
                }
            }
            catch (Exception)
            {
            }
        }

        await Task.CompletedTask;
    }

    private async Task SeedDataAsync()
    {
        var adminExists = await _db.Queryable<Admin>().AnyAsync();
        if (!adminExists)
        {
            var hashedPassword = PasswordHelper.HashPassword("Admin@2024!", out var salt);

            var defaultAdmin = new Admin
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Password = hashedPassword,
                PasswordSalt = salt,
                PermissionLevel = 99,
                RealName = "系统管理员",
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true
            };

            await _db.Insertable(defaultAdmin).ExecuteCommandAsync();
        }
    }
}
