using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Repository;
using RequestLogger.Dto;
using RequestLogger.Services;
using RequestLogger.Settings;

namespace RequestLogger.Tests.Unit;

public class RequestLoggerServiceTests
{
    private readonly RequestLoggerService _requestLoggerService;

    private readonly RequestLoggerContext _requestLoggerContext;
    
    private readonly Fixture _fixture;
        
    public RequestLoggerServiceTests()
    {
        var root = new InMemoryDatabaseRoot();
        
        var options = new DbContextOptionsBuilder<RequestLoggerContext>()
            .UseInMemoryDatabase("test", root).Options;
        _requestLoggerContext = new RequestLoggerContext(options);
        
        _requestLoggerService = new RequestLoggerService(_requestLoggerContext, new BlacklistService(Options.Create(new RequestLoggerSettings())));
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task WriteLogMessage_ReturnsLogMessage_WhenCalledCorrectly()
    {
        // Arrange
        var request = _fixture.Create<Request>();

        // Act
        var response = await _requestLoggerService.WriteLogMessage(request);

        //Assert
        response?.Body.Should().Be(request.Body);
        response?.Verb.Should().Be(request.Verb);
        response?.Customer.Should().Be(request.Customer);
    }
    
    [Fact]
    public async Task WriteLogMessage_ActuallyWritesToDatabase_WhenCalledCorrectly()
    {
        // Arrange
        var request = _fixture.Create<Request>();

        // Act
         _ = await _requestLoggerService.WriteLogMessage(request);

        //Assert
        var databaseItem = _requestLoggerContext.Requests.FirstOrDefault(r => r.Id == 1);

        databaseItem?.Body.Should().Be(request.Body);
        databaseItem?.Verb.Should().Be(request.Verb);
        databaseItem?.Customer.Should().Be(request.Customer);
    }
}