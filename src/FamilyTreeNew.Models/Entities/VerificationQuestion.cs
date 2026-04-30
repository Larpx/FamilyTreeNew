using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 验证问题实体类
/// 存储家谱访问验证问题，用于保护家谱隐私
/// </summary>
[SugarTable("VerificationQuestions")]
public class VerificationQuestion
{
    /// <summary>
    /// 问题唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "问题ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 所属家谱ID，关联FamilyTree表
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "家谱ID")]
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 验证问题内容
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false, ColumnDescription = "问题内容")]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 答案关键词，用于验证用户回答
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "答案关键词")]
    public string AnswerKeyword { get; set; } = string.Empty;

    /// <summary>
    /// 问题显示顺序，用于多问题验证时的排序
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "验证顺序")]
    public int Order { get; set; } = 1;

    /// <summary>
    /// 问题是否启用
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 问题创建时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建日期")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 导航属性：所属家谱
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(FamilyTreeId))]
    public FamilyTree? FamilyTree { get; set; }
}
