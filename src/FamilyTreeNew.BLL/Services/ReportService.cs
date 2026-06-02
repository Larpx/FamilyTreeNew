using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 报告服务
/// 生成家谱的祖先报告、后裔报告和统计报告
/// </summary>
public class ReportService : IReportService
{
    private readonly IFamilyMemberRepository _familyMemberRepository;
    private readonly IFamilyTreeRepository _familyTreeRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IFamilyMemberRepository familyMemberRepository,
        IFamilyTreeRepository familyTreeRepository,
        ILogger<ReportService> logger)
    {
        _familyMemberRepository = familyMemberRepository;
        _familyTreeRepository = familyTreeRepository;
        _logger = logger;
    }

    public async Task<AncestorReportDto> GenerateAncestorReportAsync(Guid memberId, int generations = 5)
    {
        var member = await _familyMemberRepository.GetByIdAsync(memberId);
        if (member == null)
        {
            throw new ArgumentException("成员不存在");
        }

        var ancestors = new List<AncestorNode>();

        var visited = new HashSet<Guid>();
        var membersToLoad = new List<Guid> { memberId };
        var allMembers = new Dictionary<Guid, FamilyMember>();

        while (membersToLoad.Count > 0)
        {
            var batch = membersToLoad.Where(id => !visited.Contains(id)).Distinct().ToList();
            membersToLoad.Clear();

            foreach (var id in batch)
            {
                if (visited.Contains(id)) continue;
                visited.Add(id);

                var m = await _familyMemberRepository.GetByIdAsync(id);
                if (m == null) continue;
                allMembers[id] = m;

                if (m.ParentId.HasValue && !visited.Contains(m.ParentId.Value))
                {
                    membersToLoad.Add(m.ParentId.Value);
                }
            }
        }

        CollectAncestorsInMemory(memberId, 1, generations, ancestors, allMembers);

        _logger.LogInformation("生成祖先报告，成员ID: {MemberId}，世代数: {Generations}", memberId, generations);
        return new AncestorReportDto
        {
            MemberId = memberId,
            MemberName = $"{member.Surname}{member.FirstName}",
            Generations = generations,
            Ancestors = ancestors
        };
    }

    /// <summary>
    /// 在内存中递归收集祖先节点
    /// </summary>
    private static void CollectAncestorsInMemory(Guid memberId, int generation, int maxGenerations, List<AncestorNode> ancestors, Dictionary<Guid, FamilyMember> allMembers)
    {
        if (generation > maxGenerations) return;

        if (!allMembers.TryGetValue(memberId, out var member) || member.ParentId == null) return;

        if (!allMembers.TryGetValue(member.ParentId.Value, out var parent)) return;

        var node = new AncestorNode
        {
            Id = parent.Id,
            Name = $"{parent.Surname}{parent.FirstName}",
            Generation = generation
        };
        ancestors.Add(node);

        CollectAncestorsInMemory(parent.Id, generation + 1, maxGenerations, ancestors, allMembers);
    }

    public async Task<DescendantReportDto> GenerateDescendantReportAsync(Guid memberId, int generations = 5)
    {
        var member = await _familyMemberRepository.GetByIdAsync(memberId);
        if (member == null)
        {
            throw new ArgumentException("成员不存在");
        }

        var descendants = new List<DescendantNode>();

        var memberFamilyTreeId = member.FamilyTreeId;
        var allMembers = await _familyMemberRepository.GetByFamilyTreeIdAsync(memberFamilyTreeId);
        var childrenByParent = allMembers
            .Where(m => m.ParentId.HasValue)
            .GroupBy(m => m.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        CollectDescendantsInMemory(memberId, 1, generations, descendants, childrenByParent);

        _logger.LogInformation("生成后裔报告，成员ID: {MemberId}，世代数: {Generations}", memberId, generations);
        return new DescendantReportDto
        {
            MemberId = memberId,
            MemberName = $"{member.Surname}{member.FirstName}",
            Generations = generations,
            Descendants = descendants
        };
    }

    /// <summary>
    /// 在内存中递归收集后裔节点
    /// </summary>
    private static void CollectDescendantsInMemory(Guid parentId, int generation, int maxGenerations, List<DescendantNode> nodes, Dictionary<Guid, List<FamilyMember>> childrenByParent)
    {
        if (generation > maxGenerations) return;

        if (!childrenByParent.TryGetValue(parentId, out var children)) return;

        foreach (var child in children)
        {
            var node = new DescendantNode
            {
                Id = child.Id,
                Name = $"{child.Surname}{child.FirstName}",
                Generation = generation,
                ParentId = parentId,
                Children = new List<DescendantNode>()
            };
            nodes.Add(node);

            CollectDescendantsInMemory(child.Id, generation + 1, maxGenerations, node.Children, childrenByParent);
        }
    }

    public async Task<StatisticsReportDto> GenerateStatisticsReportAsync(Guid familyTreeId)
    {
        var familyTree = await _familyTreeRepository.GetByIdAsync(familyTreeId);
        if (familyTree == null)
        {
            throw new ArgumentException("家谱不存在");
        }

        var members = await _familyMemberRepository.GetByFamilyTreeIdAsync(familyTreeId);

        var maleCount = members.Count(m => m.Gender == "M");
        var femaleCount = members.Count(m => m.Gender == "F");
        var unknownGenderCount = members.Count(m => string.IsNullOrEmpty(m.Gender));

        var maxGeneration = members.Count > 0 ? members.Max(m => m.Generation ?? 1) : 0;

        int totalAge = 0;
        int ageCount = 0;
        foreach (var member in members)
        {
            if (member.BirthDateSolar.HasValue)
            {
                DateTime endDate = member.IsDeceased && member.DeathDateSolar.HasValue
                    ? member.DeathDateSolar.Value
                    : DateTime.UtcNow;
                var age = endDate.Year - member.BirthDateSolar.Value.Year;
                if (endDate < member.BirthDateSolar.Value.AddYears(age)) age--;
                totalAge += age;
                ageCount++;
            }
        }

        var occupationDistribution = members
            .Where(m => !string.IsNullOrEmpty(m.Occupation))
            .GroupBy(m => m.Occupation!)
            .ToDictionary(g => g.Key, g => g.Count());

        var residenceDistribution = members
            .Where(m => !string.IsNullOrEmpty(m.Residence))
            .GroupBy(m => m.Residence!)
            .ToDictionary(g => g.Key, g => g.Count());

        _logger.LogInformation("生成统计报告，家谱ID: {FamilyTreeId}，成员数: {MemberCount}", familyTreeId, members.Count);
        return new StatisticsReportDto
        {
            FamilyTreeId = familyTreeId,
            FamilyTreeName = familyTree.Name,
            TotalMembers = members.Count,
            MaleCount = maleCount,
            FemaleCount = femaleCount,
            UnknownGenderCount = unknownGenderCount,
            Generations = maxGeneration,
            AverageAge = ageCount > 0 ? (double)totalAge / ageCount : 0,
            DeceasedCount = members.Count(m => m.IsDeceased),
            OccupationDistribution = occupationDistribution,
            ResidenceDistribution = residenceDistribution,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<string> GenerateHtmlReportAsync(Guid familyTreeId)
    {
        var statistics = await GenerateStatisticsReportAsync(familyTreeId);

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"zh-CN\">");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"UTF-8\">");
        html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("<title>家谱统计报告</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: 'Microsoft YaHei', sans-serif; margin: 40px; }");
        html.AppendLine("h1 { color: #333; border-bottom: 2px solid #1890ff; padding-bottom: 10px; }");
        html.AppendLine(".section { margin: 30px 0; }");
        html.AppendLine(".stat-item { display: inline-block; margin: 10px 20px; padding: 15px; background: #f5f5f5; border-radius: 8px; }");
        html.AppendLine(".stat-value { font-size: 24px; font-weight: bold; color: #1890ff; }");
        html.AppendLine(".stat-label { display: block; font-size: 14px; color: #666; }");
        html.AppendLine(".distribution-list { list-style: none; padding: 0; }");
        html.AppendLine(".distribution-item { padding: 8px; border-bottom: 1px solid #eee; }");
        html.AppendLine(".distribution-name { display: inline-block; width: 150px; }");
        html.AppendLine(".distribution-count { float: right; font-weight: bold; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine($"<h1>{statistics.FamilyTreeName} - 统计报告</h1>");
        html.AppendLine($"<p style=\"color: #666;\">生成时间: {statistics.GeneratedAt.ToString("yyyy年MM月dd日 HH:mm:ss")}</p>");

        html.AppendLine("<div class=\"section\">");
        html.AppendLine("<h2>基本统计</h2>");
        html.AppendLine($"<div class=\"stat-item\"><span class=\"stat-value\">{statistics.TotalMembers}</span><span class=\"stat-label\">总人数</span></div>");
        html.AppendLine($"<div class=\"stat-item\"><span class=\"stat-value\">{statistics.MaleCount}</span><span class=\"stat-label\">男性</span></div>");
        html.AppendLine($"<div class=\"stat-item\"><span class=\"stat-value\">{statistics.FemaleCount}</span><span class=\"stat-label\">女性</span></div>");
        html.AppendLine($"<div class=\"stat-item\"><span class=\"stat-value\">{statistics.Generations}</span><span class=\"stat-label\">世代数</span></div>");
        html.AppendLine($"<div class=\"stat-item\"><span class=\"stat-value\">{statistics.AverageAge}</span><span class=\"stat-label\">平均年龄</span></div>");
        html.AppendLine($"<div class=\"stat-item\"><span class=\"stat-value\">{statistics.DeceasedCount}</span><span class=\"stat-label\">已故人数</span></div>");
        html.AppendLine("</div>");

        if (statistics.OccupationDistribution.Any())
        {
            html.AppendLine("<div class=\"section\">");
            html.AppendLine("<h2>职业分布</h2>");
            html.AppendLine("<ul class=\"distribution-list\">");
            foreach (var item in statistics.OccupationDistribution.OrderByDescending(kv => kv.Value))
            {
                html.AppendLine($"<li class=\"distribution-item\"><span class=\"distribution-name\">{item.Key}</span><span class=\"distribution-count\">{item.Value}人</span></li>");
            }
            html.AppendLine("</ul>");
            html.AppendLine("</div>");
        }

        if (statistics.ResidenceDistribution.Any())
        {
            html.AppendLine("<div class=\"section\">");
            html.AppendLine("<h2>居住地分布</h2>");
            html.AppendLine("<ul class=\"distribution-list\">");
            foreach (var item in statistics.ResidenceDistribution.OrderByDescending(kv => kv.Value))
            {
                html.AppendLine($"<li class=\"distribution-item\"><span class=\"distribution-name\">{item.Key}</span><span class=\"distribution-count\">{item.Value}人</span></li>");
            }
            html.AppendLine("</ul>");
            html.AppendLine("</div>");
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }
}
