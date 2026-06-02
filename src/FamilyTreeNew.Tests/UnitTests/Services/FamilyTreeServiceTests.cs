using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Services;

public class FamilyTreeServiceTests
{
    private readonly Mock<IFamilyTreeRepository> _mockFamilyTreeRepository;
    private readonly FamilyTreeService _familyTreeService;

    public FamilyTreeServiceTests()
    {
        _mockFamilyTreeRepository = new Mock<IFamilyTreeRepository>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var logger = new Mock<ILogger<FamilyTreeService>>().Object;
        _familyTreeService = new FamilyTreeService(_mockFamilyTreeRepository.Object, memoryCache, logger);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {
        var familyTrees = new List<FamilyTree>
        {
            TestDataFactory.CreateTestFamilyTree(name: "Family 1"),
            TestDataFactory.CreateTestFamilyTree(name: "Family 2")
        };

        _mockFamilyTreeRepository.Setup(x => x.GetPagedWithMemberCountAsync(1, 10, null, null))
            .ReturnsAsync((familyTrees, 2));
        _mockFamilyTreeRepository.Setup(x => x.GetMemberCountsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new Dictionary<Guid, int>());

        var query = new FamilyTreeQueryDto { PageIndex = 1, PageSize = 10 };
        var result = await _familyTreeService.GetPagedAsync(query);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetPagedAsync_WithKeyword_FiltersResults()
    {
        var familyTrees = new List<FamilyTree>
        {
            TestDataFactory.CreateTestFamilyTree(name: "Test Family")
        };

        _mockFamilyTreeRepository.Setup(x => x.GetPagedWithMemberCountAsync(1, 10, "Test", null))
            .ReturnsAsync((familyTrees, 1));
        _mockFamilyTreeRepository.Setup(x => x.GetMemberCountsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new Dictionary<Guid, int>());

        var query = new FamilyTreeQueryDto { PageIndex = 1, PageSize = 10, Keyword = "Test" };
        var result = await _familyTreeService.GetPagedAsync(query);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Test Family");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsFamilyTree()
    {
        var familyTreeId = Guid.NewGuid();
        var familyTree = TestDataFactory.CreateTestFamilyTree(id: familyTreeId, name: "Test Family");

        _mockFamilyTreeRepository.Setup(x => x.GetByIdAsync(familyTreeId))
            .ReturnsAsync(familyTree);
        _mockFamilyTreeRepository.Setup(x => x.GetMemberCountAsync(familyTreeId))
            .ReturnsAsync(5);

        var result = await _familyTreeService.GetByIdAsync(familyTreeId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(familyTreeId);
        result.Name.Should().Be("Test Family");
        result.MemberCount.Should().Be(5);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var familyTreeId = Guid.NewGuid();
        _mockFamilyTreeRepository.Setup(x => x.GetByIdWithMemberCountAsync(familyTreeId))
            .ReturnsAsync((FamilyTree?)null);

        var result = await _familyTreeService.GetByIdAsync(familyTreeId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedFamilyTree()
    {
        var createDto = new FamilyTreeCreateDto
        {
            Name = "New Family Tree",
            Description = "Description",
            RequireVerification = false,
            IsEnabled = true
        };

        _mockFamilyTreeRepository.Setup(x => x.InsertAsync(It.IsAny<FamilyTree>()))
            .ReturnsAsync(1);

        var result = await _familyTreeService.CreateAsync(createDto);

        result.Should().NotBeNull();
        result.Name.Should().Be("New Family Tree");
        result.Description.Should().Be("Description");
        result.MemberCount.Should().Be(0);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ReturnsUpdatedFamilyTree()
    {
        var familyTreeId = Guid.NewGuid();
        var existingFamilyTree = TestDataFactory.CreateTestFamilyTree(id: familyTreeId);

        var updateDto = new FamilyTreeUpdateDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            RequireVerification = true,
            IsEnabled = false
        };

        _mockFamilyTreeRepository.Setup(x => x.GetByIdAsync(familyTreeId))
            .ReturnsAsync(existingFamilyTree);
        _mockFamilyTreeRepository.Setup(x => x.UpdateAsync(It.IsAny<FamilyTree>()))
            .ReturnsAsync(1);
        _mockFamilyTreeRepository.Setup(x => x.GetMemberCountAsync(familyTreeId))
            .ReturnsAsync(3);

        var result = await _familyTreeService.UpdateAsync(familyTreeId, updateDto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Description.Should().Be("Updated Description");
        result.RequireVerification.Should().BeTrue();
        result.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        var familyTreeId = Guid.NewGuid();
        var updateDto = new FamilyTreeUpdateDto { Name = "Updated Name" };

        _mockFamilyTreeRepository.Setup(x => x.GetByIdAsync(familyTreeId))
            .ReturnsAsync((FamilyTree?)null);

        var result = await _familyTreeService.UpdateAsync(familyTreeId, updateDto);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        var familyTreeId = Guid.NewGuid();
        _mockFamilyTreeRepository.Setup(x => x.ExistsAsync(familyTreeId))
            .ReturnsAsync(true);
        _mockFamilyTreeRepository.Setup(x => x.DeleteAsync(familyTreeId))
            .ReturnsAsync(1);

        var result = await _familyTreeService.DeleteAsync(familyTreeId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        var familyTreeId = Guid.NewGuid();
        _mockFamilyTreeRepository.Setup(x => x.ExistsAsync(familyTreeId))
            .ReturnsAsync(false);

        var result = await _familyTreeService.DeleteAsync(familyTreeId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ReturnsTrue()
    {
        var familyTreeId = Guid.NewGuid();
        _mockFamilyTreeRepository.Setup(x => x.ExistsAsync(familyTreeId))
            .ReturnsAsync(true);

        var result = await _familyTreeService.ExistsAsync(familyTreeId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ReturnsFalse()
    {
        var familyTreeId = Guid.NewGuid();
        _mockFamilyTreeRepository.Setup(x => x.ExistsAsync(familyTreeId))
            .ReturnsAsync(false);

        var result = await _familyTreeService.ExistsAsync(familyTreeId);

        result.Should().BeFalse();
    }
}
