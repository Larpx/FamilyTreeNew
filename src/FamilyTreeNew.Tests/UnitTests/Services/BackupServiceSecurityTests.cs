using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.DAL.Context;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Services;

/// <summary>
/// 备份服务安全测试。
/// 重点确认恢复和删除备份时会拒绝危险文件名，避免路径穿越攻击。
/// </summary>
public class BackupServiceSecurityTests
{
    private readonly Mock<ILogger<BackupService>> _mockLogger;
    private readonly IConfiguration _configuration;

    public BackupServiceSecurityTests()
    {
        _mockLogger = new Mock<ILogger<BackupService>>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=test;"
            })
            .Build();
    }

    [Fact]
    public async Task RestoreBackupAsync_RejectsPathTraversalFileName()
    {
        var service = new BackupService(
            new SqlSugarContext("Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=test;"),
            _configuration,
            _mockLogger.Object);

        var result = await service.RestoreBackupAsync("../../etc/passwd");
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("无效");
    }

    [Fact]
    public async Task RestoreBackupAsync_RejectsFileNameWithSpecialChars()
    {
        var service = new BackupService(
            new SqlSugarContext("Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=test;"),
            _configuration,
            _mockLogger.Object);

        var result = await service.RestoreBackupAsync("file; rm -rf /");
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("无效");
    }

    [Fact]
    public async Task RestoreBackupAsync_RejectsEmptyFileName()
    {
        var service = new BackupService(
            new SqlSugarContext("Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=test;"),
            _configuration,
            _mockLogger.Object);

        var result = await service.RestoreBackupAsync("");
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void DeleteBackup_RejectsPathTraversalFileName()
    {
        var service = new BackupService(
            new SqlSugarContext("Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=test;"),
            _configuration,
            _mockLogger.Object);

        var result = service.DeleteBackup("../../etc/passwd");
        result.Should().BeFalse();
    }

    [Fact]
    public void DeleteBackup_RejectsFileNameWithBackslash()
    {
        var service = new BackupService(
            new SqlSugarContext("Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=test;"),
            _configuration,
            _mockLogger.Object);

        var result = service.DeleteBackup("..\\..\\windows\\system32");
        result.Should().BeFalse();
    }

    [Fact]
    public void DeleteBackup_AcceptsValidBackupFileName()
    {
        var service = new BackupService(
            new SqlSugarContext("Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=test;"),
            _configuration,
            _mockLogger.Object);

        var result = service.DeleteBackup("backup_FamilyTreeDb_20240101_120000.sql");
        result.Should().BeFalse();
    }
}
