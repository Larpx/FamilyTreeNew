namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 分页结果通用DTO
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 当前页数据列表
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 当前页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevious => PageIndex > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNext => PageIndex < TotalPages;

    /// <summary>
    /// 是否有上一页（兼容命名）
    /// </summary>
    public bool HasPreviousPage => HasPrevious;

    /// <summary>
    /// 是否有下一页（兼容命名）
    /// </summary>
    public bool HasNextPage => HasNext;
}
