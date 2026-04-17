using System.Security.Claims;
using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.Models.Helpers;
using FamilyTreeNew.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Helpers;

public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentHashAndSaltEachTime()
    {
        var password = "TestPassword@123";

        var hash1 = PasswordHelper.HashPassword(password, out var salt1);
        var hash2 = PasswordHelper.HashPassword(password, out var salt2);

        hash1.Should().NotBeNullOrEmpty();
        hash2.Should().NotBeNullOrEmpty();
        salt1.Should().NotBeNullOrEmpty();
        salt2.Should().NotBeNullOrEmpty();
        hash1.Should().NotBe(hash2);
        salt1.Should().NotBe(salt2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        var password = "TestPassword@123";
        var hash = PasswordHelper.HashPassword(password, out var salt);

        var result = PasswordHelper.VerifyPassword(password, hash, salt);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ReturnsFalse()
    {
        var password = "CorrectPassword@123";
        var hash = PasswordHelper.HashPassword(password, out var salt);

        var result = PasswordHelper.VerifyPassword("WrongPassword@456", hash, salt);

        result.Should().BeFalse();
    }

    [Fact]
    public void GenerateRandomPassword_ReturnsCorrectLength()
    {
        var password = PasswordHelper.GenerateRandomPassword(16);

        password.Should().NotBeNullOrEmpty();
        password.Length.Should().Be(16);
    }

    [Fact]
    public void GenerateRandomPassword_DefaultLengthIs12()
    {
        var password = PasswordHelper.GenerateRandomPassword();

        password.Length.Should().Be(12);
    }

    [Fact]
    public void GenerateRandomPassword_ReturnsDifferentPasswordsEachTime()
    {
        var password1 = PasswordHelper.GenerateRandomPassword();
        var password2 = PasswordHelper.GenerateRandomPassword();

        password1.Should().NotBe(password2);
    }
}

public class XssDetectorTests
{
    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("javascript:alert(1)")]
    [InlineData("<iframe src='evil.com'>")]
    [InlineData("<object data='evil.com'>")]
    [InlineData("<embed src='evil.com'>")]
    [InlineData("vbscript:alert(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    public void ContainsXssPayload_WithMaliciousInput_ReturnsTrue(string input)
    {
        XssDetector.ContainsXssPayload(input).Should().BeTrue();
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("张三的家谱信息")]
    [InlineData("<b>合法的粗体文本</b>")]
    [InlineData("https://example.com/page?id=1&name=test")]
    [InlineData("")]
    [InlineData(null)]
    public void ContainsXssPayload_WithSafeInput_ReturnsFalse(string? input)
    {
        XssDetector.ContainsXssPayload(input).Should().BeFalse();
    }

    [Theory]
    [InlineData("onclick='alert(1)'")]
    [InlineData("onload=\"alert(1)\"")]
    [InlineData("onerror=alert(1)")]
    public void ContainsXssPayload_WithEventHandlerPayload_ReturnsTrue(string input)
    {
        XssDetector.ContainsXssPayload(input).Should().BeTrue();
    }
}

public class ConnectionStringHelperTests
{
    [Fact]
    public void ExtractDatabaseName_WithValidConnectionString_ReturnsDatabaseName()
    {
        var connectionString = "Server=localhost;Database=FamilyTreeDb;User=root;Password=123456";

        var result = ConnectionStringHelper.ExtractDatabaseName(connectionString);

        result.Should().Be("FamilyTreeDb");
    }

    [Fact]
    public void ExtractDatabaseName_WithNullConnectionString_ReturnsDefaultName()
    {
        var result = ConnectionStringHelper.ExtractDatabaseName(null);

        result.Should().Be("FamilyTreeDb");
    }

    [Fact]
    public void ExtractDatabaseName_WithEmptyConnectionString_ReturnsDefaultName()
    {
        var result = ConnectionStringHelper.ExtractDatabaseName("");

        result.Should().Be("FamilyTreeDb");
    }

    [Fact]
    public void ExtractDatabaseName_WithCustomDefaultName_ReturnsCustomDefault()
    {
        var result = ConnectionStringHelper.ExtractDatabaseName(null, "CustomDb");

        result.Should().Be("CustomDb");
    }

    [Fact]
    public void ParseConnectionString_WithFullConnectionString_ReturnsCorrectValues()
    {
        var connectionString = "Server=192.168.1.1;Port=3307;User=admin;Password=secret";

        var (host, port, user, password) = ConnectionStringHelper.ParseConnectionString(connectionString);

        host.Should().Be("192.168.1.1");
        port.Should().Be(3307);
        user.Should().Be("admin");
        password.Should().Be("secret");
    }

    [Fact]
    public void ParseConnectionString_WithMinimalConnectionString_ReturnsDefaults()
    {
        var connectionString = "Server=myhost";

        var (host, port, user, password) = ConnectionStringHelper.ParseConnectionString(connectionString);

        host.Should().Be("myhost");
        port.Should().Be(3306);
        user.Should().Be("root");
        password.Should().Be("");
    }

    [Fact]
    public void ParseConnectionString_WithUidKeyword_ReturnsCorrectUser()
    {
        var connectionString = "Server=localhost;Uid=myuser;Pwd=mypass";

        var (host, port, user, password) = ConnectionStringHelper.ParseConnectionString(connectionString);

        user.Should().Be("myuser");
        password.Should().Be("mypass");
    }
}

public class FileHelperTests
{
    [Fact]
    public void ValidateImageFile_WithValidJpg_ReturnsValid()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("photo.jpg", 1024 * 1024);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void ValidateImageFile_WithValidPng_ReturnsValid()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("photo.png", 5 * 1024 * 1024);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void ValidateImageFile_WithEmptyFile_ReturnsInvalid()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("photo.jpg", 0);

        isValid.Should().BeFalse();
        errorMessage.Should().Be("文件为空");
    }

    [Fact]
    public void ValidateImageFile_WithOversizedFile_ReturnsInvalid()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("photo.jpg", 11 * 1024 * 1024);

        isValid.Should().BeFalse();
        errorMessage.Should().Contain("文件大小超过限制");
    }

    [Fact]
    public void ValidateImageFile_WithUnsupportedExtension_ReturnsInvalid()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("document.pdf", 1024);

        isValid.Should().BeFalse();
        errorMessage.Should().Contain("不支持的文件类型");
    }

    [Theory]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    [InlineData(".png")]
    [InlineData(".gif")]
    [InlineData(".bmp")]
    [InlineData(".webp")]
    public void AllowedImageExtensions_ContainsExpectedExtensions(string extension)
    {
        FileHelper.AllowedImageExtensions.Should().Contain(extension);
    }
}

public class PhotoMapperTests
{
    [Fact]
    public void ToDto_MapsPhotoEntityToDto()
    {
        var photo = new FamilyTreeNew.Models.Entities.Photo
        {
            Id = Guid.NewGuid(),
            AlbumId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            PhotoPath = "/photos/test.jpg",
            ThumbnailPath = "/photos/thumbnails/test.jpg",
            Title = "Test Photo",
            Description = "Test Description",
            UploadedAt = DateTime.Now,
            UploadedBy = "testuser"
        };

        var dto = PhotoMapper.ToDto(photo);

        dto.Id.Should().Be(photo.Id);
        dto.AlbumId.Should().Be(photo.AlbumId);
        dto.MemberId.Should().Be(photo.MemberId);
        dto.PhotoPath.Should().Be(photo.PhotoPath);
        dto.ThumbnailPath.Should().Be(photo.ThumbnailPath);
        dto.Title.Should().Be(photo.Title);
        dto.Description.Should().Be(photo.Description);
        dto.UploadedAt.Should().Be(photo.UploadedAt);
        dto.UploadedBy.Should().Be(photo.UploadedBy);
    }

    [Fact]
    public void ToDto_WithMember_MapsMemberName()
    {
        var photo = new FamilyTreeNew.Models.Entities.Photo
        {
            Id = Guid.NewGuid(),
            AlbumId = Guid.NewGuid(),
            PhotoPath = "/photos/test.jpg",
            ThumbnailPath = "/photos/thumbnails/test.jpg",
            Title = "Test",
            UploadedAt = DateTime.Now,
            Member = new FamilyTreeNew.Models.Entities.FamilyMember
            {
                Surname = "张",
                FirstName = "三"
            }
        };

        var dto = PhotoMapper.ToDto(photo);

        dto.MemberName.Should().Be("张三");
    }

    [Fact]
    public void ToDto_WithoutMember_MapsNullMemberName()
    {
        var photo = new FamilyTreeNew.Models.Entities.Photo
        {
            Id = Guid.NewGuid(),
            AlbumId = Guid.NewGuid(),
            PhotoPath = "/photos/test.jpg",
            ThumbnailPath = "/photos/thumbnails/test.jpg",
            Title = "Test",
            UploadedAt = DateTime.Now
        };

        var dto = PhotoMapper.ToDto(photo);

        dto.MemberName.Should().BeNull();
    }
}

public class JwtHelperTests
{
    [Fact]
    public void GenerateToken_ReturnsNonNullToken()
    {
        var settings = TestDataFactory.CreateTestJwtSettings();
        var jwtHelper = new JwtHelper(settings);

        var token = jwtHelper.GenerateToken(Guid.NewGuid(), "testuser", 1);

        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsPrincipal()
    {
        var settings = TestDataFactory.CreateTestJwtSettings();
        var jwtHelper = new JwtHelper(settings);
        var adminId = Guid.NewGuid();

        var token = jwtHelper.GenerateToken(adminId, "testuser", 1);
        var principal = jwtHelper.ValidateToken(token);

        principal.Should().NotBeNull();
        principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be(adminId.ToString());
        principal.FindFirst(ClaimTypes.Name)!.Value.Should().Be("testuser");
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsNull()
    {
        var settings = TestDataFactory.CreateTestJwtSettings();
        var jwtHelper = new JwtHelper(settings);

        var principal = jwtHelper.ValidateToken("invalid.token.value");

        principal.Should().BeNull();
    }

    [Fact]
    public void GetTokenExpiration_ReturnsFutureTime()
    {
        var settings = TestDataFactory.CreateTestJwtSettings();
        var jwtHelper = new JwtHelper(settings);

        var expiration = jwtHelper.GetTokenExpiration();

        expiration.Should().BeAfter(DateTime.UtcNow);
    }
}
