using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 照片实体类
/// 存储家谱相册中的照片信息
/// </summary>
[SugarTable("Photos")]
public class Photo
{
    /// <summary>
    /// 照片唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "照片ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 所属相册ID，关联Album表
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "相册ID")]
    public Guid AlbumId { get; set; }

    /// <summary>
    /// 关联的家族成员ID，可选
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "成员ID")]
    public Guid? MemberId { get; set; }

    /// <summary>
    /// 照片存储路径
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false, ColumnDescription = "照片路径")]
    public string PhotoPath { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图存储路径，用于列表展示优化加载速度
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "缩略图路径")]
    public string? ThumbnailPath { get; set; }

    /// <summary>
    /// 照片标题
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "照片标题")]
    public string? Title { get; set; }

    /// <summary>
    /// 照片描述信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "照片描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 照片上传时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "上传日期")]
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 上传者用户名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "上传者")]
    public string? UploadedBy { get; set; }

    /// <summary>
    /// 导航属性：所属相册
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(AlbumId))]
    public Album? Album { get; set; }

    /// <summary>
    /// 导航属性：关联的家族成员
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(MemberId))]
    public FamilyMember? Member { get; set; }
}
