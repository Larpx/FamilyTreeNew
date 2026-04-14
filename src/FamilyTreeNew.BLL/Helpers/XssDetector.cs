using System.Text.RegularExpressions;

namespace FamilyTreeNew.BLL.Helpers;

/// <summary>
/// XSS攻击检测辅助类，通过正则表达式匹配常见的XSS攻击载荷模式
/// </summary>
public static class XssDetector
{
    /// <summary>
    /// XSS攻击特征正则表达式集合，涵盖script标签、事件处理器、iframe、javascript协议等常见攻击模式
    /// </summary>
    private static readonly Regex[] XssPatterns =
    [
        new(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"javascript\s*:", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"on\w+\s*=\s*[""'][^""']*[""']", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"on\w+\s*=", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"<iframe[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"<object[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"<embed[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"expression\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"vbscript\s*:", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"data\s*:\s*text/html", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1))
    ];

    /// <summary>
    /// 检测输入字符串是否包含XSS攻击载荷
    /// </summary>
    /// <param name="input">待检测的输入字符串，可为null</param>
    /// <returns>如果检测到XSS载荷返回true，否则返回false</returns>
    public static bool ContainsXssPayload(string? input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        foreach (var pattern in XssPatterns)
        {
            try
            {
                if (pattern.IsMatch(input))
                {
                    return true;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return true;
            }
        }

        return false;
    }
}
