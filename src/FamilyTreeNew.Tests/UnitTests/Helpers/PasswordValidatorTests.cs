using FamilyTreeNew.BLL.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Helpers;

/// <summary>
/// PasswordValidator 单元测试
/// 测试密码策略验证的各种场景
/// </summary>
public class PasswordValidatorTests
{
    private readonly PasswordValidator _validator;

    public PasswordValidatorTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Security:PasswordMinLength", "8" },
            { "Security:RequireUppercase", "true" },
            { "Security:RequireLowercase", "true" },
            { "Security:RequireDigit", "true" },
            { "Security:RequireSpecialChar", "true" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        _validator = new PasswordValidator(configuration);
    }

    [Fact]
    public void Validate_ValidPassword_ReturnsSuccess()
    {
        var result = _validator.Validate("Strong@Pass123");

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_TooShortPassword_ReturnsFailure()
    {
        var result = _validator.Validate("Ab@1");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("密码长度"));
    }

    [Fact]
    public void Validate_MissingUppercase_ReturnsFailure()
    {
        var result = _validator.Validate("lower@pass123");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("大写字母"));
    }

    [Fact]
    public void Validate_MissingLowercase_ReturnsFailure()
    {
        var result = _validator.Validate("UPPER@PASS123");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("小写字母"));
    }

    [Fact]
    public void Validate_MissingDigit_ReturnsFailure()
    {
        var result = _validator.Validate("NoDigit@Pass");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("数字"));
    }

    [Fact]
    public void Validate_MissingSpecialChar_ReturnsFailure()
    {
        var result = _validator.Validate("NoSpecial123Pass");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("特殊字符"));
    }

    [Fact]
    public void Validate_NullPassword_ReturnsFailure()
    {
        var result = _validator.Validate(null);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("密码不能为空"));
    }

    [Fact]
    public void Validate_EmptyPassword_ReturnsFailure()
    {
        var result = _validator.Validate("");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("密码不能为空"));
    }

    [Fact]
    public void Validate_CommonPatternPassword_ReturnsFailure()
    {
        var result = _validator.Validate("Password@12345678");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("常见弱密码模式"));
    }

    [Fact]
    public void Validate_TooLongPassword_ReturnsFailure()
    {
        var longPassword = new string('A', 130) + "@a1";
        var result = _validator.Validate(longPassword);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("128"));
    }

    [Fact]
    public void Validate_ValidPassword_HasStrengthScore()
    {
        var result = _validator.Validate("Strong@Pass123");

        result.IsValid.Should().BeTrue();
        result.StrengthScore.Should().BeGreaterThan(0);
        result.StrengthLevel.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void StaticValidate_WithDefaultParams_ValidatesCorrectly()
    {
        var result = PasswordValidator.Validate("Strong@Pass123");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StaticValidate_WithCustomMinLength_ValidatesCorrectly()
    {
        var result = PasswordValidator.Validate("Sh@1", minLength: 4);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StaticValidate_WithoutSpecialCharRequirement_ValidatesCorrectly()
    {
        var result = PasswordValidator.Validate("NoSpecial123Pass", requireSpecialChar: false);

        result.IsValid.Should().BeTrue();
    }
}
