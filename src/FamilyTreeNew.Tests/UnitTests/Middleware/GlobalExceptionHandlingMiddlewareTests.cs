using FamilyTreeNew.Api.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Middleware;

/// <summary>
/// 全局异常处理中间件测试。
/// 用于验证不同异常类型会被转换成正确的 HTTP 状态码。
/// </summary>
public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlingMiddleware>> _mockLogger;
    private readonly Mock<IHostEnvironment> _mockEnv;

    public GlobalExceptionHandlingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        _mockEnv = new Mock<IHostEnvironment>();
        _mockEnv.Setup(e => e.EnvironmentName).Returns("Development");
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextMiddleware()
    {
        var nextCalled = false;
        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            _mockLogger.Object,
            _mockEnv.Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_GenericException_Returns500()
    {
        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new Exception("Test error"),
            _mockLogger.Object,
            _mockEnv.Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_ArgumentException_Returns400()
    {
        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new ArgumentException("Invalid argument"),
            _mockLogger.Object,
            _mockEnv.Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_UnauthorizedAccessException_Returns401()
    {
        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new UnauthorizedAccessException("Not authorized"),
            _mockLogger.Object,
            _mockEnv.Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_InvalidOperationException_Returns400()
    {
        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Invalid operation"),
            _mockLogger.Object,
            _mockEnv.Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}