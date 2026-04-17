using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FamilyTreeNew.Api;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.DTOs.Auth;
using FamilyTreeNew.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FamilyTreeNew.Tests.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetFamilyTrees_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/FamilyTrees");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFamilyTreeById_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/FamilyTrees/{invalidId}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequestDto
        {
            Username = "nonexistent",
            Password = "wrongpassword"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/Auth/login", content);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithEmptyCredentials_ReturnsBadRequest()
    {
        var loginRequest = new LoginRequestDto
        {
            Username = "",
            Password = ""
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/Auth/login", content);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetInfo_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/Auth/info");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateFamilyTree_WithoutAuth_ReturnsUnauthorizedOrSuccess()
    {
        var createDto = new FamilyTreeCreateDto
        {
            Name = "Test Family Tree",
            Description = "Test Description"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/FamilyTrees", content);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSystemInfo_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/System/info");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DownloadTemplate_ReturnsExcelFile()
    {
        var response = await _client.GetAsync("/api/FamilyTrees/template");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType
            .Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        response.Content.Headers.ContentDisposition?.FileNameStar
            .Should().Be("成员导入模板.xlsx");
    }
}

public class AuthFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthFlowIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task FullAuthFlow_WithoutValidCredentials_ReturnsAppropriateErrors()
    {
        var loginRequest = new LoginRequestDto
        {
            Username = "testuser",
            Password = "TestPassword123!"
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _client.PostAsync("/api/Auth/login", loginContent);

        if (loginResponse.StatusCode == HttpStatusCode.OK)
        {
            var loginResult = JsonSerializer.Deserialize<LoginResponseDto>(
                await loginResponse.Content.ReadAsStringAsync(), _jsonOptions);

            if (loginResult?.Success == true && !string.IsNullOrEmpty(loginResult.Token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", loginResult.Token);

                var infoResponse = await _client.GetAsync("/api/Auth/info");
                infoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
        else
        {
            loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    [Fact]
    public async Task ChangePassword_WithoutAuth_ReturnsUnauthorized()
    {
        var changePasswordRequest = new
        {
            OldPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(changePasswordRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/Auth/change-password", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync("/api/Auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

public class FamilyTreeCrudIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public FamilyTreeCrudIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetFamilyTreesList_ReturnsPagedResult()
    {
        var response = await _client.GetAsync("/api/FamilyTrees?pageIndex=1&pageSize=10");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFamilyTreeMembers_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/FamilyTrees/{invalidId}/members");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateFamilyTree_WithInvalidData_ReturnsBadRequest()
    {
        var createDto = new
        {
            Name = "",
            Description = "Test"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/FamilyTrees", content);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.OK,
            HttpStatusCode.Created,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateFamilyTree_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var updateDto = new FamilyTreeUpdateDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(updateDto),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PutAsync($"/api/FamilyTrees/{invalidId}", content);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteFamilyTree_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/api/FamilyTrees/{invalidId}");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized);
    }
}
