using FamilyTreeNew.Api.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FamilyTreeNew.Tests.UnitTests.Middleware;

public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlingMiddleware>> _mockLogger;

    public GlobalExceptionHandlingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextMiddleware()
    {
        var nextCalled = false;
        var middleware = new GlobalExceptionHandlingMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            _mockLogger.Object);

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
            _mockLogger.Object);

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
            _mockLogger.Object);

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
            _mockLogger.Object);

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
            _mockLogger.Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
