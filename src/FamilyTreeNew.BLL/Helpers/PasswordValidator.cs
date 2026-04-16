using System.Text.RegularExpressions;

namespace FamilyTreeNew.BLL.Helpers;

public class PasswordValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();
    public int StrengthScore { get; set; }
    public string StrengthLevel => StrengthScore switch
    {
        >= 80 => "强",
        >= 60 => "中",
        >= 40 => "弱",
        _ => "非常弱"
    };
}

public static class PasswordValidator
{
    private static readonly Regex UppercasePattern = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowercasePattern = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex DigitPattern = new(@"[0-9]", RegexOptions.Compiled);
    private static readonly Regex SpecialCharPattern = new(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]", RegexOptions.Compiled);
    private static readonly Regex CommonPatterns = new(@"(password|123456|qwerty|admin|root|user)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static PasswordValidationResult Validate(string? password, int minLength = 8, bool requireUppercase = true,
        bool requireLowercase = true, bool requireDigit = true, bool requireSpecialChar = true)
    {
        var result = new PasswordValidationResult();

        if (string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Errors.Add("密码不能为空");
            return result;
        }

        if (password.Length < minLength)
        {
            result.IsValid = false;
            result.Errors.Add($"密码长度至少需要{minLength}个字符");
        }

        if (password.Length > 128)
        {
            result.IsValid = false;
            result.Errors.Add("密码长度不能超过128个字符");
        }

        if (requireUppercase && !UppercasePattern.IsMatch(password))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个大写字母");
        }

        if (requireLowercase && !LowercasePattern.IsMatch(password))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个小写字母");
        }

        if (requireDigit && !DigitPattern.IsMatch(password))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个数字");
        }

        if (requireSpecialChar && !SpecialCharPattern.IsMatch(password))
        {
            result.IsValid = false;
            result.Errors.Add("密码必须包含至少一个特殊字符（如 !@#$%^&*）");
        }

        if (CommonPatterns.IsMatch(password))
        {
            result.IsValid = false;
            result.Errors.Add("密码包含常见弱密码模式，请使用更复杂的密码");
        }

        result.StrengthScore = CalculateStrength(password);

        return result;
    }

    private static int CalculateStrength(string password)
    {
        int score = 0;

        score += Math.Min(password.Length * 4, 40);

        if (UppercasePattern.IsMatch(password)) score += 10;
        if (LowercasePattern.IsMatch(password)) score += 10;
        if (DigitPattern.IsMatch(password)) score += 10;
        if (SpecialCharPattern.IsMatch(password)) score += 15;

        if (UppercasePattern.Matches(password).Count >= 2) score += 5;
        if (LowercasePattern.Matches(password).Count >= 2) score += 5;
        if (DigitPattern.Matches(password).Count >= 2) score += 5;
        if (SpecialCharPattern.Matches(password).Count >= 2) score += 5;

        if (HasMixedCase(password)) score += 5;
        if (HasNumbersAndLetters(password)) score += 5;

        if (CommonPatterns.IsMatch(password)) score -= 20;

        if (HasRepeatingChars(password)) score -= 10;

        return Math.Max(0, Math.Min(100, score));
    }

    private static bool HasMixedCase(string password)
    {
        return UppercasePattern.IsMatch(password) && LowercasePattern.IsMatch(password);
    }

    private static bool HasNumbersAndLetters(string password)
    {
        return DigitPattern.IsMatch(password) && (UppercasePattern.IsMatch(password) || LowercasePattern.IsMatch(password));
    }

    private static bool HasRepeatingChars(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] == password[i + 1] && password[i] == password[i + 2])
            {
                return true;
            }
        }
        return false;
    }
}
