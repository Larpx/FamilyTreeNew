using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Services;

/// <summary>
/// VerificationService 单元测试
/// 测试访问令牌的生成和验证逻辑
/// </summary>
public class VerificationServiceTests
{
    private readonly Mock<IVerificationQuestionRepository> _mockQuestionRepository;
    private readonly Mock<IFamilyTreeRepository> _mockFamilyTreeRepository;
    private readonly IConfiguration _configuration;
    private readonly VerificationService _service;

    public VerificationServiceTests()
    {
        _mockQuestionRepository = new Mock<IVerificationQuestionRepository>();
        _mockFamilyTreeRepository = new Mock<IFamilyTreeRepository>();

        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Verification:TokenExpirationHours", "24" }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _service = new VerificationService(
            _mockQuestionRepository.Object,
            _mockFamilyTreeRepository.Object,
            _configuration);
    }

    [Fact]
    public void GenerateAccessToken_ReturnsValidToken()
    {
        var familyTreeId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        var token = _service.GenerateAccessToken(familyTreeId, questionId);

        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ValidateAccessToken_MatchingFamilyTreeId_ReturnsTrue()
    {
        var familyTreeId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var token = _service.GenerateAccessToken(familyTreeId, questionId);

        var result = _service.ValidateAccessToken(token, familyTreeId);

        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateAccessToken_NonMatchingFamilyTreeId_ReturnsFalse()
    {
        var familyTreeId = Guid.NewGuid();
        var otherFamilyTreeId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var token = _service.GenerateAccessToken(familyTreeId, questionId);

        var result = _service.ValidateAccessToken(token, otherFamilyTreeId);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateAccessToken_ExpiredToken_ReturnsFalse()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Verification:TokenExpirationHours", "0" }
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        var service = new VerificationService(
            _mockQuestionRepository.Object,
            _mockFamilyTreeRepository.Object,
            config);

        var familyTreeId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var token = service.GenerateAccessToken(familyTreeId, questionId);

        var result = service.ValidateAccessToken(token, familyTreeId);

        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateAccessToken_InvalidToken_ReturnsFalse()
    {
        var familyTreeId = Guid.NewGuid();

        var result = _service.ValidateAccessToken("invalid-token-value", familyTreeId);

        result.Should().BeFalse();
    }

    [Fact]
    public void GenerateAccessToken_DifferentCalls_ReturnDifferentTokens()
    {
        var familyTreeId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        var token1 = _service.GenerateAccessToken(familyTreeId, questionId);
        var token2 = _service.GenerateAccessToken(familyTreeId, questionId);

        token1.Should().NotBe(token2);
    }

    [Fact]
    public async Task VerifyAnswerAsync_NonExistentFamilyTree_ReturnsFailure()
    {
        var familyTreeId = Guid.NewGuid();
        _mockFamilyTreeRepository.Setup(x => x.GetByIdAsync(familyTreeId))
            .ReturnsAsync((FamilyTree?)null);

        var dto = new VerifyAnswerDto
        {
            FamilyTreeId = familyTreeId,
            QuestionId = Guid.NewGuid(),
            Answer = "test"
        };

        var result = await _service.VerifyAnswerAsync(dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("家谱不存在");
    }

    [Fact]
    public async Task VerifyAnswerAsync_FamilyTreeWithoutVerification_ReturnsSuccess()
    {
        var familyTreeId = Guid.NewGuid();
        var familyTree = new FamilyTree
        {
            Id = familyTreeId,
            Name = "测试家谱",
            RequireVerification = false,
            IsEnabled = true
        };
        _mockFamilyTreeRepository.Setup(x => x.GetByIdAsync(familyTreeId))
            .ReturnsAsync(familyTree);

        var dto = new VerifyAnswerDto
        {
            FamilyTreeId = familyTreeId,
            QuestionId = Guid.NewGuid(),
            Answer = "test"
        };

        var result = await _service.VerifyAnswerAsync(dto);

        result.Success.Should().BeTrue();
        result.AllQuestionsPassed.Should().BeTrue();
    }
}
