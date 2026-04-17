namespace FamilyTreeNew.Web.Models;

/// <summary>
/// 错误页面视图模型。
/// 该模型把请求标识、状态码和用户可读的错误说明传递给 Razor 视图。
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// 当前请求的标识。
    /// 发生错误时，开发人员可以用它快速定位问题请求。
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// HTTP 状态码。
    /// 例如 404、401 或 500。
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// 页面标题。
    /// 默认会显示为一个更友好的中文提示。
    /// </summary>
    public string Title { get; set; } = "页面出错了";

    /// <summary>
    /// 错误说明文字。
    /// 页面会把这段文字直接展示给用户。
    /// </summary>
    public string Message { get; set; } = "请求处理过程中发生异常，请稍后重试。";

    /// <summary>
    /// 是否显示请求标识。
    /// 只要 RequestId 不为空，页面就会展示给用户。
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
