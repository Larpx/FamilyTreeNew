using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Services;

public class FamilyMemberServiceTests
{
    private readonly Mock<IFamilyMemberRepository> _mockMemberRepository;
    private readonly Mock<IFamilyTreeRepository> _mockFamilyTreeRepository;
    private readonly FamilyMemberService _memberService;

    public FamilyMemberServiceTests()
    {
        _mockMemberRepository = new Mock<IFamilyMemberRepository>();
        _mockFamilyTreeRepository = new Mock<IFamilyTreeRepository>();
        _memberService = new FamilyMemberService(_mockMemberRepository.Object, _mockFamilyTreeRepository.Object);
    }

    [Fact]
    public async Task GetPagedAsync_WithValidFamilyTreeId_ReturnsPagedResult()
    {
        var familyTreeId = Guid.NewGuid();
        var members = new List<FamilyMember>
        {
            TestDataFactory.CreateTestFamilyMember(familyTreeId: familyTreeId, surname: "张", firstName: "三"),
            TestDataFactory.CreateTestFamilyMember(familyTreeId: familyTreeId, surname: "张", firstName: "四")
        };

        _mockMemberRepository.Setup(x => x.GetPagedByFamilyTreeAsync(familyTreeId, 1, 20, null, null, null))
            .ReturnsAsync((members, 2));
        _mockMemberRepository.Setup(x => x.GetParentNamesAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new Dictionary<Guid, string>());

        var query = new FamilyMemberQueryDto { FamilyTreeId = familyTreeId, PageIndex = 1, PageSize = 20 };
        var result = await _memberService.GetPagedAsync(query);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetPagedAsync_WithoutFamilyTreeId_ReturnsEmptyResult()
    {
        var query = new FamilyMemberQueryDto { PageIndex = 1, PageSize = 20 };
        var result = await _memberService.GetPagedAsync(query);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsMember()
    {
        var memberId = Guid.NewGuid();
        var familyTreeId = Guid.NewGuid();
        var parent = TestDataFactory.CreateTestFamilyMember(id: Guid.NewGuid(), familyTreeId: familyTreeId, surname: "张", firstName: "父");
        var member = TestDataFactory.CreateTestFamilyMember(id: memberId, familyTreeId: familyTreeId, parentId: parent.Id, surname: "张", firstName: "子");
        member.Parent = parent;

        _mockMemberRepository.Setup(x => x.GetByIdWithParentAsync(memberId))
            .ReturnsAsync(member);

        var result = await _memberService.GetByIdAsync(memberId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(memberId);
        result.FullName.Should().Be("张子");
        result.ParentName.Should().Be("张父");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var memberId = Guid.NewGuid();
        _mockMemberRepository.Setup(x => x.GetByIdWithParentAsync(memberId))
            .ReturnsAsync((FamilyMember?)null);

        var result = await _memberService.GetByIdAsync(memberId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithoutParent_ReturnsMemberWithGeneration1()
    {
        var familyTreeId = Guid.NewGuid();
        var createDto = new FamilyMemberCreateDto
        {
            FamilyTreeId = familyTreeId,
            Surname = "张",
            FirstName = "三",
            BirthDateSolar = new DateTime(1990, 1, 1),
            Residence = "北京市"
        };

        _mockMemberRepository.Setup(x => x.InsertAsync(It.IsAny<FamilyMember>()))
            .ReturnsAsync(1);

        var result = await _memberService.CreateAsync(createDto);

        result.Should().NotBeNull();
        result.Surname.Should().Be("张");
        result.FirstName.Should().Be("三");
        result.Generation.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithParent_ReturnsMemberWithCorrectGeneration()
    {
        var familyTreeId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var createDto = new FamilyMemberCreateDto
        {
            FamilyTreeId = familyTreeId,
            ParentId = parentId,
            Surname = "张",
            FirstName = "子"
        };

        _mockMemberRepository.Setup(x => x.GetGenerationByParentIdAsync(parentId))
            .ReturnsAsync(1);
        _mockMemberRepository.Setup(x => x.InsertAsync(It.IsAny<FamilyMember>()))
            .ReturnsAsync(1);

        var result = await _memberService.CreateAsync(createDto);

        result.Should().NotBeNull();
        result.Generation.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ReturnsUpdatedMember()
    {
        var memberId = Guid.NewGuid();
        var familyTreeId = Guid.NewGuid();
        var existingMember = TestDataFactory.CreateTestFamilyMember(id: memberId, familyTreeId: familyTreeId);

        var updateDto = new FamilyMemberUpdateDto
        {
            Surname = "李",
            FirstName = "四",
            Residence = "上海市"
        };

        _mockMemberRepository.Setup(x => x.GetByIdAsync(memberId))
            .ReturnsAsync(existingMember);
        _mockMemberRepository.Setup(x => x.UpdateAsync(It.IsAny<FamilyMember>()))
            .ReturnsAsync(1);

        var result = await _memberService.UpdateAsync(memberId, updateDto);

        result.Should().NotBeNull();
        result!.Surname.Should().Be("李");
        result.FirstName.Should().Be("四");
        result.Residence.Should().Be("上海市");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        var memberId = Guid.NewGuid();
        var updateDto = new FamilyMemberUpdateDto { Surname = "李", FirstName = "四" };

        _mockMemberRepository.Setup(x => x.GetByIdAsync(memberId))
            .ReturnsAsync((FamilyMember?)null);

        var result = await _memberService.UpdateAsync(memberId, updateDto);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithValidIdAndNoChildren_ReturnsTrue()
    {
        var memberId = Guid.NewGuid();
        _mockMemberRepository.Setup(x => x.ExistsAsync(memberId))
            .ReturnsAsync(true);
        _mockMemberRepository.Setup(x => x.HasChildrenAsync(memberId))
            .ReturnsAsync(false);
        _mockMemberRepository.Setup(x => x.DeleteAsync(memberId))
            .ReturnsAsync(1);

        var result = await _memberService.DeleteAsync(memberId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithChildren_ThrowsInvalidOperationException()
    {
        var memberId = Guid.NewGuid();
        _mockMemberRepository.Setup(x => x.ExistsAsync(memberId))
            .ReturnsAsync(true);
        _mockMemberRepository.Setup(x => x.HasChildrenAsync(memberId))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _memberService.DeleteAsync(memberId));
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        var memberId = Guid.NewGuid();
        _mockMemberRepository.Setup(x => x.ExistsAsync(memberId))
            .ReturnsAsync(false);

        var result = await _memberService.DeleteAsync(memberId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByFamilyTreeIdAsync_ReturnsAllMembers()
    {
        var familyTreeId = Guid.NewGuid();
        var members = new List<FamilyMember>
        {
            TestDataFactory.CreateTestFamilyMember(familyTreeId: familyTreeId),
            TestDataFactory.CreateTestFamilyMember(familyTreeId: familyTreeId)
        };

        _mockMemberRepository.Setup(x => x.GetByFamilyTreeIdAsync(familyTreeId))
            .ReturnsAsync(members);

        var result = await _memberService.GetByFamilyTreeIdAsync(familyTreeId);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CalculateGenerationAsync_WithoutParent_Returns1()
    {
        var result = await _memberService.CalculateGenerationAsync(null);

        result.Should().Be(1);
    }

    [Fact]
    public async Task CalculateGenerationAsync_WithParent_ReturnsCorrectGeneration()
    {
        var parentId = Guid.NewGuid();
        _mockMemberRepository.Setup(x => x.GetGenerationByParentIdAsync(parentId))
            .ReturnsAsync(3);

        var result = await _memberService.CalculateGenerationAsync(parentId);

        result.Should().Be(4);
    }

    [Fact]
    public async Task CalculateGenerationAsync_WithNonExistentParent_ThrowsArgumentException()
    {
        var parentId = Guid.NewGuid();
        _mockMemberRepository.Setup(x => x.GetGenerationByParentIdAsync(parentId))
            .ReturnsAsync((int?)null);

        await Assert.ThrowsAsync<ArgumentException>(() => _memberService.CalculateGenerationAsync(parentId));
    }
}
