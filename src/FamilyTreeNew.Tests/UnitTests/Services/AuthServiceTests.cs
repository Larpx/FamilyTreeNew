using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs.Auth;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Models.Helpers;
using FamilyTreeNew.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IAdminRepository> _mockAdminRepository;
    private readonly Mock<IOperationLogRepository> _mockOperationLogRepository;
    private readonly Mock<IJwtHelper> _mockJwtHelper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockAdminRepository = new Mock<IAdminRepository>();
        _mockOperationLogRepository = new Mock<IOperationLogRepository>();
        _mockJwtHelper = new Mock<IJwtHelper>();
        _authService = new AuthService(
            _mockAdminRepository.Object,
            _mockOperationLogRepository.Object,
            _mockJwtHelper.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResponse()
    {
        var admin = TestDataFactory.CreateTestAdmin();
        var hashedPassword = PasswordHelper.HashPassword("Test@123456", out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        _mockAdminRepository.Setup(x => x.GetByUsernameAsync("testuser"))
            .ReturnsAsync(admin);
        _mockJwtHelper.Setup(x => x.GenerateToken(admin.Id, admin.Username))
            .Returns("test-token");
        _mockJwtHelper.Setup(x => x.GetTokenExpiration())
            .Returns(DateTime.UtcNow.AddHours(2));
        _mockAdminRepository.Setup(x => x.UpdateLastLoginTimeAsync(admin.Id))
            .ReturnsAsync(1);
        _mockOperationLogRepository.Setup(x => x.InsertAsync(It.IsAny<OperationLog>()))
            .ReturnsAsync(1);

        var request = new LoginRequestDto { Username = "testuser", Password = "Test@123456" };
        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("登录成功");
        result.Token.Should().Be("test-token");
        result.AdminInfo.Should().NotBeNull();
        result.AdminInfo!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ReturnsFailureResponse()
    {
        _mockAdminRepository.Setup(x => x.GetByUsernameAsync("nonexistent"))
            .ReturnsAsync((Admin?)null);
        _mockOperationLogRepository.Setup(x => x.InsertAsync(It.IsAny<OperationLog>()))
            .ReturnsAsync(1);

        var request = new LoginRequestDto { Username = "nonexistent", Password = "password" };
        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("用户名或密码错误");
        result.Token.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithDisabledAccount_ReturnsFailureResponse()
    {
        var admin = TestDataFactory.CreateTestAdmin(isEnabled: false);
        _mockAdminRepository.Setup(x => x.GetByUsernameAsync("testuser"))
            .ReturnsAsync(admin);
        _mockOperationLogRepository.Setup(x => x.InsertAsync(It.IsAny<OperationLog>()))
            .ReturnsAsync(1);

        var request = new LoginRequestDto { Username = "testuser", Password = "Test@123456" };
        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("账户已被禁用，请联系管理员");
    }

    [Fact]
    public async Task LoginAsync_WithEmptyPasswordSalt_ReturnsFailureResponse()
    {
        var admin = TestDataFactory.CreateTestAdmin();
        admin.PasswordSalt = null;

        _mockAdminRepository.Setup(x => x.GetByUsernameAsync("testuser"))
            .ReturnsAsync(admin);
        _mockOperationLogRepository.Setup(x => x.InsertAsync(It.IsAny<OperationLog>()))
            .ReturnsAsync(1);

        var request = new LoginRequestDto { Username = "testuser", Password = "Test@123456" };
        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("账户配置异常，请联系管理员");
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFailureResponse()
    {
        var admin = TestDataFactory.CreateTestAdmin();
        var hashedPassword = PasswordHelper.HashPassword("CorrectPassword@123", out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        _mockAdminRepository.Setup(x => x.GetByUsernameAsync("testuser"))
            .ReturnsAsync(admin);
        _mockOperationLogRepository.Setup(x => x.InsertAsync(It.IsAny<OperationLog>()))
            .ReturnsAsync(1);

        var request = new LoginRequestDto { Username = "testuser", Password = "WrongPassword@123" };
        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("用户名或密码错误");
    }

    [Fact]
    public async Task GetAdminByIdAsync_WithValidId_ReturnsAdmin()
    {
        var adminId = Guid.NewGuid();
        var admin = TestDataFactory.CreateTestAdmin(id: adminId);

        _mockAdminRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(admin);

        var result = await _authService.GetAdminByIdAsync(adminId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(adminId);
        result.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetAdminByIdAsync_WithInvalidId_ReturnsNull()
    {
        var adminId = Guid.NewGuid();
        _mockAdminRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync((Admin?)null);

        var result = await _authService.GetAdminByIdAsync(adminId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_ReturnsSuccess()
    {
        var adminId = Guid.NewGuid();
        var admin = TestDataFactory.CreateTestAdmin(id: adminId);
        var hashedPassword = PasswordHelper.HashPassword("OldPassword@123", out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        _mockAdminRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(admin);
        _mockAdminRepository.Setup(x => x.UpdateAsync(It.IsAny<Admin>()))
            .ReturnsAsync(1);
        _mockOperationLogRepository.Setup(x => x.InsertAsync(It.IsAny<OperationLog>()))
            .ReturnsAsync(1);

        var (success, message) = await _authService.ChangePasswordAsync(adminId, "OldPassword@123", "NewPass@456");

        success.Should().BeTrue();
        message.Should().Be("密码修改成功");
    }

    [Fact]
    public async Task ChangePasswordAsync_WithNonExistentUser_ReturnsFailure()
    {
        var adminId = Guid.NewGuid();
        _mockAdminRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync((Admin?)null);

        var (success, message) = await _authService.ChangePasswordAsync(adminId, "OldPassword@123", "NewPassword@123");

        success.Should().BeFalse();
        message.Should().Be("用户不存在");
    }

    [Fact]
    public async Task ChangePasswordAsync_WithWrongOldPassword_ReturnsFailure()
    {
        var adminId = Guid.NewGuid();
        var admin = TestDataFactory.CreateTestAdmin(id: adminId);
        var hashedPassword = PasswordHelper.HashPassword("CorrectOldPassword@123", out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        _mockAdminRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(admin);

        var (success, message) = await _authService.ChangePasswordAsync(adminId, "WrongOldPassword@123", "NewPassword@123");

        success.Should().BeFalse();
        message.Should().Be("原密码错误");
    }

    [Fact]
    public async Task ChangePasswordAsync_WithWeakNewPassword_ReturnsFailure()
    {
        var adminId = Guid.NewGuid();
        var admin = TestDataFactory.CreateTestAdmin(id: adminId);
        var hashedPassword = PasswordHelper.HashPassword("OldPassword@123", out var salt);
        admin.Password = hashedPassword;
        admin.PasswordSalt = salt;

        _mockAdminRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(admin);

        var (success, message) = await _authService.ChangePasswordAsync(adminId, "OldPassword@123", "weak");

        success.Should().BeFalse();
        message.Should().Contain("密码长度");
    }
}
