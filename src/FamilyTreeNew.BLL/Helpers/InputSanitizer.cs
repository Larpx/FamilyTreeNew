using System.Text;
using System.Text.RegularExpressions;

namespace FamilyTreeNew.BLL.Helpers;

public static class InputSanitizer
{
    private static readonly Regex HtmlTagPattern = new(@"<[^>]*>", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex ScriptPattern = new(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex JsEventPattern = new(@"on\w+\s*=\s*[""'][^""']*[""']", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex JsProtocolPattern = new(@"javascript\s*:", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex VbsProtocolPattern = new(@"vbscript\s*:", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex DataUrlPattern = new(@"data\s*:\s*text/html", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public static string SanitizeHtml(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        var sanitized = input;

        sanitized = ScriptPattern.Replace(sanitized, string.Empty);
        sanitized = JsEventPattern.Replace(sanitized, string.Empty);
        sanitized = JsProtocolPattern.Replace(sanitized, string.Empty);
        sanitized = VbsProtocolPattern.Replace(sanitized, string.Empty);
        sanitized = DataUrlPattern.Replace(sanitized, string.Empty);

        return sanitized.Trim();
    }

    public static string StripHtmlTags(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        return HtmlTagPattern.Replace(input, string.Empty).Trim();
    }

    public static string EscapeHtml(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        var result = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            switch (c)
            {
                case '<':
                    result.Append("&lt;");
                    break;
                case '>':
                    result.Append("&gt;");
                    break;
                case '&':
                    result.Append("&amp;");
                    break;
                case '"':
                    result.Append("&quot;");
                    break;
                case '\'':
                    result.Append("&#39;");
                    break;
                default:
                    result.Append(c);
                    break;
            }
        }

        return result.ToString();
    }

    public static string SanitizeFileName(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new StringBuilder(fileName.Length);

        foreach (var c in fileName)
        {
            if (!invalidChars.Contains(c) && c != '.' && c != ' ')
            {
                sanitized.Append(c);
            }
        }

        return sanitized.ToString();
    }

    public static string SanitizePath(string? path)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;

        var invalidChars = Path.GetInvalidPathChars();
        var sanitized = new StringBuilder(path.Length);

        foreach (var c in path)
        {
            if (!invalidChars.Contains(c))
            {
                sanitized.Append(c);
            }
        }

        var result = sanitized.ToString();

        if (result.Contains("..") || result.Contains("~"))
        {
            return string.Empty;
        }

        return result;
    }

    public static bool ContainsXssPayload(string? input) => XssDetector.ContainsXssPayload(input);

    public static bool ContainsSqlInjection(string? input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        var sqlKeywords = new[]
        {
            "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "UNION", "EXEC", "EXECUTE",
            "XP_", "SP_", "TRUNCATE", "ALTER", "CREATE", "DESTROY", "--", "/*", "*/", ";--"
        };

        var upperInput = input.ToUpperInvariant();

        return sqlKeywords.Any(keyword => upperInput.Contains(keyword));
    }

    public static string Truncate(string? input, int maxLength)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        return input.Length <= maxLength ? input : input[..maxLength];
    }

    public static string NormalizeWhitespace(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        return Regex.Replace(input, @"\s+", " ").Trim();
    }
}
