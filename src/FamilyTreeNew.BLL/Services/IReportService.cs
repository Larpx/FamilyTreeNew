using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IReportService
{
    Task<AncestorReportDto> GenerateAncestorReportAsync(Guid memberId, int generations = 5);
    
    Task<DescendantReportDto> GenerateDescendantReportAsync(Guid memberId, int generations = 5);
    
    Task<StatisticsReportDto> GenerateStatisticsReportAsync(Guid familyTreeId);
    
    Task<string> GenerateHtmlReportAsync(Guid familyTreeId);
}

public class AncestorReportDto
{
    public Guid MemberId { get; set; }
    
    public string MemberName { get; set; } = string.Empty;
    
    public int Generations { get; set; }
    
    public List<AncestorNode> Ancestors { get; set; } = new List<AncestorNode>();
}

public class AncestorNode
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int Generation { get; set; }
    
    public Guid? FatherId { get; set; }
    
    public Guid? MotherId { get; set; }
}

public class DescendantReportDto
{
    public Guid MemberId { get; set; }
    
    public string MemberName { get; set; } = string.Empty;
    
    public int Generations { get; set; }
    
    public List<DescendantNode> Descendants { get; set; } = new List<DescendantNode>();
}

public class DescendantNode
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int Generation { get; set; }
    
    public Guid ParentId { get; set; }
    
    public List<DescendantNode> Children { get; set; } = new List<DescendantNode>();
}

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