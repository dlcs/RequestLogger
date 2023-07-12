using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RequestLogger.Dto;
using RequestLogger.Services;
using RequestLogger.Settings;

namespace RequestLogger.Tests.Unit;

public class BlacklistServiceTests
{
    private readonly BlacklistService _blacklistService;
    private readonly Fixture _fixture;
    
    public BlacklistServiceTests()
    {
        var settings = new RequestLoggerSettings()
        {
            BlacklistSettings = new BlacklistSettings()
            {
                EndpointBlacklist = new List<string>()
                {
                    "/blacklist/path"
                },
                HeaderBlacklist = new List<string>()
                {
                    "blacklist"
                },
                BodyStorageBlacklist = new List<string>()
                {
                    "/blacklist/body"
                },
                QueryParamBlacklist = new List<string>()
                {
                    "blacklist"
                }
            }
        };
        
        _blacklistService = new BlacklistService(Options.Create(settings));
        _fixture = new Fixture();
    }
    
    [Fact]
    public void CheckBlacklist_ReturnsUnmodifiedObject_WhenCalledCorrectly()
    {
        // Arrange
        var request = _fixture.Create<Request>();

        // Act
        var checkedBlacklist = _blacklistService.CheckBlacklist(request);

        //Assert
        checkedBlacklist.DoNotStore.Should().BeFalse();
        checkedBlacklist.request?.Body.Should().Be(request.Body);
        checkedBlacklist.request?.Headers.Should().Contain(request.Headers);
        checkedBlacklist.request?.Path.Should().Be(request.Path);
        checkedBlacklist.request?.QueryParams.Should().Contain(request.QueryParams);
    }
    
    [Fact]
    public void CheckBlacklist_ReturnsDoNotStoreAsTrue_WhenCalledWithBlacklistedPath()
    {
        // Arrange
        var request = _fixture.
            Build<Request>()
            .With(r => r.Path, "/blacklist/path")
            .Create();

        // Act
        var checkedBlacklist = _blacklistService.CheckBlacklist(request);

        //Assert
        checkedBlacklist.DoNotStore.Should().BeTrue();
        checkedBlacklist.request.Should().BeNull();
    }
    
    [Fact]
    public void CheckBlacklist_ReturnsWithoutHeader_WhenCalledWithBlacklistedHeader()
    {
        // Arrange
        var headers = new Dictionary<string, string> { { "blacklist", "whoCares" } };

        var request = _fixture.
            Build<Request>()
            .With(r => r.Headers, headers)
            .Create();

        // Act
        var checkedBlacklist = _blacklistService.CheckBlacklist(request);

        //Assert
        checkedBlacklist.DoNotStore.Should().BeFalse();
        checkedBlacklist.request?.Body.Should().Be(request.Body);
        checkedBlacklist.request?.Headers.Count.Should().Be(0);
        checkedBlacklist.request?.Path.Should().Be(request.Path);
        checkedBlacklist.request?.QueryParams.Should().Contain(request.QueryParams);
    }
    
    [Fact]
    public void CheckBlacklist_ReturnsWithRemovedBody_WhenCalledWithBlacklistedBodyStoragePath()
    {
        // Arrange
        var request = _fixture.
            Build<Request>()
            .With(r => r.Path, "/blacklist/body")
            .Create();

        // Act
        var checkedBlacklist = _blacklistService.CheckBlacklist(request);

        //Assert
        checkedBlacklist.DoNotStore.Should().BeFalse();
        checkedBlacklist.request?.Body.Should().Be("{ \"jsonRemoved\": \"json\" }");
        checkedBlacklist.request?.Headers.Should().Contain(request.Headers);
        checkedBlacklist.request?.Path.Should().Be(request.Path);
        checkedBlacklist.request?.QueryParams.Should().Contain(request.QueryParams);
    }
    
    [Fact]
    public void CheckBlacklist_ReturnsWithRemovedQueryParam_WhenCalledWithBlacklistedQueryParam()
    {
        // Arrange
        var queryParams = new Dictionary<string, string> { { "blacklist", "whoCares" } };

        var request = _fixture.
            Build<Request>()
            .With(r => r.QueryParams, queryParams)
            .Create();

        // Act
        var checkedBlacklist = _blacklistService.CheckBlacklist(request);

        //Assert
        checkedBlacklist.DoNotStore.Should().BeFalse();
        checkedBlacklist.request?.Body.Should().Be(request.Body);
        checkedBlacklist.request?.Headers.Should().Contain(request.Headers);
        checkedBlacklist.request?.Path.Should().Be(request.Path);
        checkedBlacklist.request?.QueryParams?.Count.Should().Be(0);
    }
}