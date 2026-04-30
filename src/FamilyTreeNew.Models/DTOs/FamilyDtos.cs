using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

public class FamilyCreateRequestDto
{
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    [Required(ErrorMessage = "家族名称不能为空")]
    [StringLength(100, ErrorMessage = "家族名称不能超过100个字符")]
    public string FamilyName { get; set; } = string.Empty;

    public Guid? HeadMemberId { get; set; }

    [StringLength(500, ErrorMessage = "家庭地址不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(2000, ErrorMessage = "家族描述不能超过2000个字符")]
    public string? Description { get; set; }
}

public class FamilyUpdateRequestDto
{
    [Required(ErrorMessage = "家族名称不能为空")]
    [StringLength(100, ErrorMessage = "家族名称不能超过100个字符")]
    public string FamilyName { get; set; } = string.Empty;

    public Guid? HeadMemberId { get; set; }

    [StringLength(500, ErrorMessage = "家庭地址不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(2000, ErrorMessage = "家族描述不能超过2000个字符")]
    public string? Description { get; set; }
}

public class FamilyResponseDto
{
    public int Id { get; set; }

    public Guid FamilyTreeId { get; set; }

    public string FamilyName { get; set; } = string.Empty;

    public Guid? HeadMemberId { get; set; }

    public string? HeadMemberName { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
