using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Services;

/// <summary>
/// GEDCOM 导入导出服务单元测试
/// 重点验证父子关系在导出/导入往返过程中不被丢失
/// </summary>
public class GedcomServiceTests
{
    private readonly Mock<IFamilyTreeRepository> _mockFamilyTreeRepository;
    private readonly Mock<IFamilyMemberRepository> _mockFamilyMemberRepository;
    private readonly Mock<IFamilyMemberService> _mockFamilyMemberService;
    private readonly GedcomService _gedcomService;

    public GedcomServiceTests()
    {
        _mockFamilyTreeRepository = new Mock<IFamilyTreeRepository>();
        _mockFamilyMemberRepository = new Mock<IFamilyMemberRepository>();
        _mockFamilyMemberService = new Mock<IFamilyMemberService>();
        var logger = new Mock<ILogger<GedcomService>>().Object;

        _gedcomService = new GedcomService(
            _mockFamilyTreeRepository.Object,
            _mockFamilyMemberRepository.Object,
            _mockFamilyMemberService.Object,
            logger);
    }

    [Fact]
    public async Task ExportToGedcomAsync_WithParentChildRelation_UsesIndiReferenceForFamc()
    {
        // 排列：构造父子成员，父在子之前（与仓储按 Generation 升序返回一致）
        var familyTreeId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var familyTree = TestDataFactory.CreateTestFamilyTree(id: familyTreeId);
        var parent = TestDataFactory.CreateTestFamilyMember(id: parentId, familyTreeId: familyTreeId, surname: "张", firstName: "父");
        var child = TestDataFactory.CreateTestFamilyMember(id: childId, familyTreeId: familyTreeId, parentId: parentId, surname: "张", firstName: "子");

        _mockFamilyTreeRepository.Setup(x => x.GetByIdAsync(familyTreeId))
            .ReturnsAsync(familyTree);
        _mockFamilyMemberRepository.Setup(x => x.GetByFamilyTreeIdAsync(familyTreeId))
            .ReturnsAsync(new List<FamilyMember> { parent, child });

        // 行动：导出家谱为 GEDCOM
        var gedcom = await _gedcomService.ExportToGedcomAsync(familyTreeId);

        // 断言：FAMC 引用必须使用 @I...@ 前缀（与导入端 memberMap 的键一致），
        // 否则导入时 TryGetValue 永远查不到，父子关系会被静默丢失
        gedcom.Should().Contain("1 FAMC @I");
        gedcom.Should().NotContain("1 FAMC @F");
        // 导出的 FAMC 引用应包含父成员 Id（去掉连字符）
        var expectedRef = $"1 FAMC @I{parentId.ToString().Replace("-", "")}@";
        gedcom.Should().Contain(expectedRef);
    }

    [Fact]
    public async Task ImportFromGedcomAsync_WithExportedContent_PreservesParentChildRelation()
    {
        // 排列：先构造一份导出的 GEDCOM 内容（父在前、子在后，FAMC 指向父 INDI 引用）
        var familyTreeId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var familyTree = TestDataFactory.CreateTestFamilyTree(id: familyTreeId);
        var parent = TestDataFactory.CreateTestFamilyMember(id: parentId, familyTreeId: familyTreeId, surname: "张", firstName: "父");
        var child = TestDataFactory.CreateTestFamilyMember(id: childId, familyTreeId: familyTreeId, parentId: parentId, surname: "张", firstName: "子");

        _mockFamilyTreeRepository.Setup(x => x.GetByIdAsync(familyTreeId))
            .ReturnsAsync(familyTree);
        _mockFamilyMemberRepository.Setup(x => x.GetByFamilyTreeIdAsync(familyTreeId))
            .ReturnsAsync(new List<FamilyMember> { parent, child });

        var gedcomContent = await _gedcomService.ExportToGedcomAsync(familyTreeId);

        // 捕获导入时创建的成员，验证 ParentId 是否正确传递
        var createdMembers = new List<FamilyMemberCreateDto>();
        _mockFamilyMemberService.Setup(x => x.CreateAsync(It.IsAny<FamilyMemberCreateDto>()))
            .Callback<FamilyMemberCreateDto>(dto => createdMembers.Add(dto))
            .ReturnsAsync((FamilyMemberCreateDto dto) => new FamilyMemberDto { Id = Guid.NewGuid(), Surname = dto.Surname, FirstName = dto.FirstName });

        // 行动：导入刚导出的 GEDCOM 内容
        var (success, _) = await _gedcomService.ImportFromGedcomAsync(familyTreeId, gedcomContent);

        // 断言：导入成功，且子的 ParentId 被正确设置（不为空）
        success.Should().BeTrue();
        createdMembers.Should().HaveCount(2);
        var importedChild = createdMembers.SingleOrDefault(m => m.FirstName == "子");
        importedChild.Should().NotBeNull();
        importedChild!.ParentId.Should().NotBeNull(
            "导出再导入必须保留父子关系；若 FAMC 引用前缀与 memberMap 键不一致，ParentId 会被静默置空");
    }
}
