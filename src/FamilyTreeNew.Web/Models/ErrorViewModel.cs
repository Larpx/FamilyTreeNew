namespace FamilyTreeNew.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public int? StatusCode { get; set; }

    public string Title { get; set; } = "页面出错了";

    public string Message { get; set; } = "请求处理过程中发生异常，请稍后重试。";

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
