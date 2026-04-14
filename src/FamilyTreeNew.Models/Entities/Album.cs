using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 相册实体类
/// 存储家谱相册信息，一个家谱可以有多个相册
/// </summary>
[SugarTable("Albums")]
public class Album
{
    /// <summary>
    /// 相册唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "相册ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 所属家谱ID，关联FamilyTree表
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "家谱ID")]
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 相册名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "相册名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 相册描述信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "相册描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 相册创建时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建日期")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 相册最后更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新日期")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 导航属性：所属家谱
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(FamilyTreeId))]
    public FamilyTree? FamilyTree { get; set; }

    /// <summary>
    /// 导航属性：相册中的照片列表
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(Photo.AlbumId))]
    public List<Photo>? Photos { get; set; }
}
