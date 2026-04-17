using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建事件类型时使用的请求模型。
/// 用于定义某类事件的名称、编码、描述和排序信息。
/// </summary>
public class EventTypeCreateRequestDto
{
    [Required(ErrorMessage = "事件类型名称不能为空")]
    [StringLength(50, ErrorMessage = "事件类型名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "事件类型编码不能为空")]
    [StringLength(50, ErrorMessage = "事件类型编码不能超过50个字符")]
    [RegularExpression(@"^[A-Za-z_][A-Za-z0-9_]*$", ErrorMessage = "事件类型编码只能包含字母、数字和下划线")]
    public string Code { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "描述不能超过200个字符")]
    public string? Description { get; set; }

    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int SortOrder { get; set; } = 0;

    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 更新事件类型时使用的请求模型。
/// 主要用于修改事件类型名称、描述和启用状态。
/// </summary>
public class EventTypeUpdateRequestDto
{
    [Required(ErrorMessage = "事件类型名称不能为空")]
    [StringLength(50, ErrorMessage = "事件类型名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "描述不能超过200个字符")]
    public string? Description { get; set; }

    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; }
}

/// <summary>
/// 事件类型返回模型。
/// 前端可以用它展示可用的事件分类列表。
/// </summary>
public class EventTypeResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 创建事件时使用的请求模型。
/// 这个模型把事件类型、成员、地点和日期等信息一次性提交给后端。
/// </summary>
public class EventCreateRequestDto
{
    [Required(ErrorMessage = "事件类型ID不能为空")]
    public Guid EventTypeId { get; set; }

    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    [Required(ErrorMessage = "成员ID不能为空")]
    public Guid MemberId { get; set; }

    public Guid? PlaceId { get; set; }

    public DateTime? DateSolar { get; set; }

    [StringLength(50, ErrorMessage = "农历日期不能超过50个字符")]
    public string? DateLunar { get; set; }

    [StringLength(2000, ErrorMessage = "描述不能超过2000个字符")]
    public string? Description { get; set; }

    public bool IsPrimary { get; set; } = false;

    [StringLength(2000, ErrorMessage = "备注不能超过2000个字符")]
    public string? Remarks { get; set; }
}

/// <summary>
/// 更新事件时使用的请求模型。
/// 适合在后台编辑已经录入的事件资料。
/// </summary>
public class EventUpdateRequestDto
{
    [Required(ErrorMessage = "事件类型ID不能为空")]
    public Guid EventTypeId { get; set; }

    public Guid? PlaceId { get; set; }

    public DateTime? DateSolar { get; set; }

    [StringLength(50, ErrorMessage = "农历日期不能超过50个字符")]
    public string? DateLunar { get; set; }

    [StringLength(2000, ErrorMessage = "描述不能超过2000个字符")]
    public string? Description { get; set; }

    public bool IsPrimary { get; set; }

    [StringLength(2000, ErrorMessage = "备注不能超过2000个字符")]
    public string? Remarks { get; set; }
}

/// <summary>
/// 事件返回模型。
/// 前端通过它显示某条事件的完整内容和关联对象名称。
/// </summary>
public class EventResponseDto
{
    public Guid Id { get; set; }

    public Guid EventTypeId { get; set; }

    public string EventTypeName { get; set; } = string.Empty;

    public Guid FamilyTreeId { get; set; }

    public Guid MemberId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public Guid? PlaceId { get; set; }

    public string? PlaceName { get; set; }

    public DateTime? DateSolar { get; set; }

    public string? DateLunar { get; set; }

    public string? Description { get; set; }

    public bool IsPrimary { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
