namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 报表服务接口。
/// 负责生成祖先报表、后代报表、统计报表和 HTML 报表。
/// </summary>
public interface IReportService
{
    Task<AncestorReportDto> GenerateAncestorReportAsync(Guid memberId, int generations = 5);

    Task<DescendantReportDto> GenerateDescendantReportAsync(Guid memberId, int generations = 5);

    Task<StatisticsReportDto> GenerateStatisticsReportAsync(Guid familyTreeId);

    Task<string> GenerateHtmlReportAsync(Guid familyTreeId);
}

/// <summary>
/// 祖先报表数据模型。
/// 用来保存某个成员的祖先树和报表生成参数。
/// </summary>
public class AncestorReportDto
{
    public Guid MemberId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public int Generations { get; set; }

    public List<AncestorNode> Ancestors { get; set; } = new List<AncestorNode>();
}

/// <summary>
/// 祖先节点模型。
/// 表示祖先树中的一个节点，以及它的父母引用。
/// </summary>
public class AncestorNode
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Generation { get; set; }

    public Guid? FatherId { get; set; }

    public Guid? MotherId { get; set; }
}

/// <summary>
/// 后代报表数据模型。
/// 用来保存某个成员的后代树和报表生成参数。
/// </summary>
public class DescendantReportDto
{
    public Guid MemberId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public int Generations { get; set; }

    public List<DescendantNode> Descendants { get; set; } = new List<DescendantNode>();
}

/// <summary>
/// 后代节点模型。
/// 表示后代树中的一个节点，以及它的子节点集合。
/// </summary>
public class DescendantNode
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Generation { get; set; }

    public Guid ParentId { get; set; }

    public List<DescendantNode> Children { get; set; } = new List<DescendantNode>();
}

/// <summary>
/// 统计报表数据模型。
/// 用来汇总家谱中的成员数量、性别分布、年龄和职业分布等信息。
/// </summary>
public class StatisticsReportDto
{
    public Guid FamilyTreeId { get; set; }

    public string FamilyTreeName { get; set; } = string.Empty;

    public int TotalMembers { get; set; }

    public int MaleCount { get; set; }

    public int FemaleCount { get; set; }

    public int UnknownGenderCount { get; set; }

    public int Generations { get; set; }

    public double AverageAge { get; set; }

    public int DeceasedCount { get; set; }

    public Dictionary<string, int> OccupationDistribution { get; set; } = new Dictionary<string, int>();

    public Dictionary<string, int> ResidenceDistribution { get; set; } = new Dictionary<string, int>();

    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}