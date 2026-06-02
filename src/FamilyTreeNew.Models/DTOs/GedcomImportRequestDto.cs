using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// GEDCOM导入请求DTO
/// </summary>
public class GedcomImportRequestDto
{
    /// <summary>
    /// GEDCOM文件内容
    /// </summary>
    [Required(ErrorMessage = "GEDCOM内容不能为空")]
    public string Content { get; set; } = string.Empty;
}
