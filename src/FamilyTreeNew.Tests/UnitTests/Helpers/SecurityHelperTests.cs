using FamilyTreeNew.BLL.Helpers;
using FluentAssertions;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Helpers;

/// <summary>
/// 输入安全工具测试。
/// 主要验证 SQL 注入检测、HTML 清理和路径清洗规则是否正确。
/// </summary>
public class SecurityHelperTests
{
    [Fact]
    public void InputSanitizer_ContainsSqlInjection_DetectsSelectKeyword()
    {
        var result = InputSanitizer.ContainsSqlInjection("SELECT * FROM users");
        result.Should().BeTrue();
    }

    [Fact]
    public void InputSanitizer_ContainsSqlInjection_ReturnsFalseForNormalText()
    {
        var result = InputSanitizer.ContainsSqlInjection("张三");
        result.Should().BeFalse();
    }

    [Fact]
    public void InputSanitizer_ContainsSqlInjection_ReturnsFalseForEmptyInput()
    {
        var result = InputSanitizer.ContainsSqlInjection("");
        result.Should().BeFalse();
    }

    [Fact]
    public void InputSanitizer_ContainsSqlInjection_ReturnsFalseForNullInput()
    {
        var result = InputSanitizer.ContainsSqlInjection(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void InputSanitizer_StripHtmlTags_RemovesAllHtmlTags()
    {
        var result = InputSanitizer.StripHtmlTags("<p>Hello</p><br/>");
        result.Should().Be("Hello");
    }

    [Fact]
    public void InputSanitizer_EscapeHtml_EscapesSpecialCharacters()
    {
        var result = InputSanitizer.EscapeHtml("<script>alert('xss')</script>");
        result.Should().NotContain("<").And.NotContain(">");
    }

    [Fact]
    public void InputSanitizer_SanitizePath_RejectsPathTraversal()
    {
        var result = InputSanitizer.SanitizePath("../../etc/passwd");
        result.Should().BeEmpty();
    }

    [Fact]
    public void InputSanitizer_SanitizePath_RejectsTildePath()
    {
        var result = InputSanitizer.SanitizePath("~/secret");
        result.Should().BeEmpty();
    }

    [Fact]
    public void XssDetector_ContainsXssPayload_DetectsScriptTag()
    {
        var result = XssDetector.ContainsXssPayload("<script>alert('xss')</script>");
        result.Should().BeTrue();
    }

    [Fact]
    public void XssDetector_ContainsXssPayload_DetectsJavascriptProtocol()
    {
        var result = XssDetector.ContainsXssPayload("javascript:alert(1)");
        result.Should().BeTrue();
    }

    [Fact]
    public void XssDetector_ContainsXssPayload_DetectsEventHandler()
    {
        var result = XssDetector.ContainsXssPayload("onerror=\"alert(1)\"");
        result.Should().BeTrue();
    }

    [Fact]
    public void XssDetector_ContainsXssPayload_ReturnsFalseForNormalText()
    {
        var result = XssDetector.ContainsXssPayload("Hello World");
        result.Should().BeFalse();
    }

    [Fact]
    public void XssDetector_ContainsXssPayload_ReturnsFalseForNullInput()
    {
        var result = XssDetector.ContainsXssPayload(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void FileHelper_ValidateImageFile_AcceptsValidJpg()
    {
        var (isValid, _) = FileHelper.ValidateImageFile("photo.jpg", 1024);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void FileHelper_ValidateImageFile_AcceptsValidPng()
    {
        var (isValid, _) = FileHelper.ValidateImageFile("photo.png", 1024);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void FileHelper_ValidateImageFile_RejectsEmptyFile()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("photo.jpg", 0);
        isValid.Should().BeFalse();
        errorMessage.Should().Contain("空");
    }

    [Fact]
    public void FileHelper_ValidateImageFile_RejectsOversizedFile()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("photo.jpg", FileHelper.MaxImageFileSize + 1);
        isValid.Should().BeFalse();
        errorMessage.Should().Contain("超过限制");
    }

    [Fact]
    public void FileHelper_ValidateImageFile_RejectsInvalidExtension()
    {
        var (isValid, errorMessage) = FileHelper.ValidateImageFile("malware.exe", 1024);
        isValid.Should().BeFalse();
        errorMessage.Should().Contain("不支持");
    }

    [Fact]
    public void PasswordValidator_Validate_RejectsWeakPassword()
    {
        var result = PasswordValidator.Validate("weak");
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void PasswordValidator_Validate_AcceptsStrongPassword()
    {
        var result = PasswordValidator.Validate("Strong@Pass123");
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void PasswordValidator_Validate_RejectsCommonPattern()
    {
        var result = PasswordValidator.Validate("Password@12345678");
        result.IsValid.Should().BeFalse();
    }
}
