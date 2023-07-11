using System.Text.Json;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using RequestLogger.Dto;
using RequestLogger.Services;
using RequestLogger.Services.Interfaces;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RequestLogger.Tests.Helpers;

#pragma warning disable CS8620

namespace RequestLogger.Tests.Unit;

public class ProgramTests
{
    private readonly WebApplicationFactory<Program> _sut;

    public ProgramTests()
    {
        var root = new InMemoryDatabaseRoot();
        
        _sut = new RequestLoggerAppBuilderFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<RequestLoggerContext>));
                
                services.Remove(descriptor!);
                services.AddScoped<IRequestLoggerService, RequestLoggerService>();
                services.AddScoped<IBlacklistService, BlacklistService>();
                services.AddDbContext<RequestLoggerContext>(x => x.UseInMemoryDatabase("Testing", root));
            }));
    }
    
    [Fact]
    public async Task Program_CallingGet_ReturnsResponse()
    {
        // Arrange
        var client = _sut.CreateClient();

        // Act
        var response = await client.GetAsync("/stuff/");
        var responseBody = await response.Content.ReadAsStringAsync(); 
        
        //Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        responseBody.Should().Contain("GET");
    }
    
    [Fact]
    public async Task Program_MakingCall_ReturnsResponseFromDatabase()
    {
        // Arrange
        var client = _sut.CreateClient();

        
        
        // Act
        var response = await client.GetAsync("/stuff/");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var parsedContent = JsonSerializer.Deserialize<Request>(responseContent);
        
        //Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        parsedContent?.Customer.Should().BeNull();
        parsedContent?.Body.Should().BeNull();
        parsedContent?.Verb.Should().Be("GET");
        parsedContent?.QueryParams.Should().BeNull();
    }
    
    [Fact]
    public async Task Program_MakingCall_ReturnsCustomerSuccessfully()
    {
        // Arrange
        var client = _sut.CreateClient();

        // Act
        var response = await client.GetAsync("/stuff/customer/3425234");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var parsedContent = JsonSerializer.Deserialize<Request>(responseContent);
        
        //Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        parsedContent?.Customer.Should().Be("3425234");
        parsedContent?.Body.Should().BeNull();
        parsedContent?.Verb.Should().Be("GET");
        parsedContent?.QueryParams.Should().BeNull();
    }

    [Fact] 
    public async Task Program_MakingCall_ReturnsQueryParamsSuccessfully()
    {
        // Arrange
        var client = _sut.CreateClient();

        // Act
        var response = await client.GetAsync("/some/url/3425234?query=something");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var parsedContent = JsonSerializer.Deserialize<Request>(responseContent);
        
        //Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        parsedContent?.Customer.Should().BeNull();
        parsedContent?.Body.Should().BeNull();
        parsedContent?.Verb.Should().Be("GET");
        parsedContent?.QueryParams.Should().ContainKey("query");
    }
    
    [Fact] 
    public async Task Program_MakingPostCall_ReturnsCorrectVerb()
    {
        // Arrange
        var client = _sut.CreateClient();

        // Act
        var response = await client.PostAsync("/some/uri", new StringContent(""));
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var parsedContent = JsonSerializer.Deserialize<Request>(responseContent);
        
        //Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        parsedContent?.Customer.Should().BeNull();
        parsedContent?.Body.Should().BeNull();
        parsedContent?.Verb.Should().Be("POST");
        parsedContent?.QueryParams.Should().BeNull();
    }
    
    [Fact] 
    public async Task Program_MakingPostCallWithJsonBody_ReturnsCorrectBody()
    {
        // Arrange
        var client = _sut.CreateClient();
        var jsonBody = "{\"test\": \"test\"}";

        // Act
        var response = await client.PostAsync("/some/uri", new StringContent(jsonBody));
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var parsedContent = JsonSerializer.Deserialize<Request>(responseContent);
        
        //Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        parsedContent?.Customer.Should().BeNull();
        parsedContent?.Body.Should().Be(jsonBody);
        parsedContent?.Verb.Should().Be("POST");
        parsedContent?.QueryParams.Should().BeNull();
    }
    
    [Fact] 
    public async Task Program_MakingPostCallWithInvalidJsonBody_ReturnsCorrectBody()
    {
        // Arrange
        var client = _sut.CreateClient();
        var requestBody = "test";
        var jsonBody = "{ \"invalidJson\": \"test\" }";
        
        // Act
        var response = await client.PostAsync("/some/uri", new StringContent(requestBody));
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var parsedContent = JsonSerializer.Deserialize<Request>(responseContent);
        
        //Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        parsedContent?.Customer.Should().BeNull();
        parsedContent?.Body.Should().Be(jsonBody);
        parsedContent?.Verb.Should().Be("POST");
        parsedContent?.QueryParams.Should().BeNull();
    }
}