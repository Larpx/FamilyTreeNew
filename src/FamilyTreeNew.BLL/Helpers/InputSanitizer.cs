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
    private static readonly Regex[] SqlInjectionPatterns =
    {
        new(@"\b(SELECT|INSERT|UPDATE|DELETE|DROP|TRUNCATE|ALTER|CREATE|EXEC|EXECUTE|UNION)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"'\s*OR\s*'[^']*'\s*=\s*'[^']*'", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"'\s*;\s*--", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@";\s*(DROP|DELETE|TRUNCATE|ALTER)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"\bXP_[A-Z0-9_]+\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
        new(@"\b0x[0-9A-F]+\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1)),
    };

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
            if (!invalidChars.Contains(c))
            {
                sanitized.Append(c);
            }
        }

        var result = sanitized.ToString();

        if (result.StartsWith(".") || result.EndsWith("."))
        {
            result = result.Trim('.');
        }

        if (string.IsNullOrEmpty(result))
        {
            return "unnamed_file";
        }

        return result;
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

        foreach (var pattern in SqlInjectionPatterns)
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
                continue;
            }
        }

        return false;
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
