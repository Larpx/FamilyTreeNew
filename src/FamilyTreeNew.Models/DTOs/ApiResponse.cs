namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 统一API响应模型，封装接口返回数据、状态码和错误信息
/// </summary>
/// <typeparam name="T">响应数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// HTTP状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 错误详情列表
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// 创建成功响应
    /// </summary>
    /// <param name="data">响应数据</param>
    /// <param name="message">响应消息</param>
    /// <returns>成功的ApiResponse实例</returns>
    public static ApiResponse<T> Ok(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Code = 200
        };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">HTTP状态码</param>
    /// <param name="errors">错误详情列表</param>
    /// <returns>失败的ApiResponse实例</returns>
    public static ApiResponse<T> Fail(string message, int code = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Code = code,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// 无泛型API响应模型，用于不需要返回数据的场景
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// 创建无数据的成功响应
    /// </summary>
    /// <param name="message">响应消息</param>
    /// <returns>成功的ApiResponse实例</returns>
    public static ApiResponse Ok(string message = "操作成功")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Code = 200
        };
    }

    /// <summary>
    /// 创建无数据的失败响应
    /// 通过 new 关键字返回非泛型 ApiResponse 而非基类的 ApiResponse&lt;object&gt;，
    /// 使调用方无需指定泛型参数即可获得简洁的响应对象
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">HTTP状态码</param>
    /// <param name="errors">错误详情列表</param>
    /// <returns>失败的ApiResponse实例</returns>
    public new static ApiResponse Fail(string message, int code = 400, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Code = code,
            Errors = errors ?? new List<string>()
        };
    }
}
