namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// Excel导入结果DTO
/// </summary>
public class ExcelImportResultDto
{
    /// <summary>
    /// 是否导入成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 结果消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 总行数
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// 成功导入行数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 已导入行数（同SuccessCount）
    /// </summary>
    public int ImportedCount => SuccessCount;

    /// <summary>
    /// 跳过行数（如重复数据）
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// 失败行数
    /// </summary>
    public int FailCount { get; set; }

    /// <summary>
    /// 错误详情列表
    /// </summary>
    public List<ExcelImportErrorDto> Errors { get; set; } = new();
}

/// <summary>
/// Excel导入错误详情DTO
/// </summary>
public class ExcelImportErrorDto
{
    /// <summary>
    /// 行号
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// 字段名
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 行数据（JSON格式）
    /// </summary>
    public string? RowData { get; set; }
}
