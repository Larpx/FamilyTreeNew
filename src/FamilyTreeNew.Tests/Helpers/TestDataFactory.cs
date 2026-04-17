using FamilyTreeNew.BLL.Configuration;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.Tests.Helpers;

public static class TestDataFactory
{
    public static Admin CreateTestAdmin(Guid? id = null, string username = "testuser", string password = "Test@123456", bool isEnabled = true)
    {
        var salt = "testSalt12345678901234==";
        return new Admin
        {
            Id = id ?? Guid.NewGuid(),
            Username = username,
            Password = password,
            PasswordSalt = salt,
            PermissionLevel = 1,
            RealName = "Test User",
            Email = "test@example.com",
            CreatedAt = DateTime.Now,
            IsEnabled = isEnabled
        };
    }

    public static FamilyTree CreateTestFamilyTree(Guid? id = null, string name = "Test Family Tree")
    {
        return new FamilyTree
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Description = "Test Description",
            CreatedAt = DateTime.Now,
            RequireVerification = false,
            IsEnabled = true
        };
    }

    public static FamilyMember CreateTestFamilyMember(Guid? id = null, Guid? familyTreeId = null, Guid? parentId = null, string surname = "张", string firstName = "三")
    {
        return new FamilyMember
        {
            Id = id ?? Guid.NewGuid(),
            FamilyTreeId = familyTreeId ?? Guid.NewGuid(),
            ParentId = parentId,
            Generation = parentId.HasValue ? 2 : 1,
            Surname = surname,
            FirstName = firstName,
            Alias = "别名",
            Ranking = "1",
            GenerationName = "字辈",
            BirthDateSolar = new DateTime(1990, 1, 1),
            BirthDateLunar = "1990年正月初一",
            Residence = "北京市",
            Occupation = "工程师",
            PersonalInfo = "个人信息",
            Note = "小注",
            IsDeceased = false,
            CreatedAt = DateTime.Now
        };
    }

    public static Album CreateTestAlbum(Guid? id = null, Guid? familyTreeId = null, string name = "Test Album")
    {
        return new Album
        {
            Id = id ?? Guid.NewGuid(),
            FamilyTreeId = familyTreeId ?? Guid.NewGuid(),
            Name = name,
            Description = "Test Album Description",
            CreatedAt = DateTime.Now
        };
    }

    public static Photo CreateTestPhoto(Guid? id = null, Guid? albumId = null, Guid? memberId = null)
    {
        return new Photo
        {
            Id = id ?? Guid.NewGuid(),
            AlbumId = albumId ?? Guid.NewGuid(),
            MemberId = memberId,
            PhotoPath = "/photos/test.jpg",
            ThumbnailPath = "/photos/thumbnails/test.jpg",
            Title = "Test Photo",
            Description = "Test Description",
            UploadedAt = DateTime.Now,
            UploadedBy = "testuser"
        };
    }

    public static OperationLog CreateTestOperationLog(Guid? id = null, Guid? adminId = null)
    {
        return new OperationLog
        {
            Id = id ?? Guid.NewGuid(),
            AdminId = adminId,
            OperationType = "Login",
            Module = "Auth",
            Content = "Test operation",
            OperationTime = DateTime.Now,
            IpAddress = "127.0.0.1"
        };
    }

    public static JwtSettings CreateTestJwtSettings()
    {
        return new JwtSettings
        {
            SecretKey = "ThisIsATestSecretKeyThatIsAtLeast32CharactersLong!",
            Issuer = "FamilyTreeNew",
            Audience = "FamilyTreeNewUsers",
            ExpirationMinutes = 120
        };
    }
}
