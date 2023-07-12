using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using RequestLogger.Dto;
using RequestLogger.Services.Interfaces;
using RequestLogger.Settings;

namespace RequestLogger.Services;

public class BlacklistService : IBlacklistService
{
    private const string BodyBlacklistJson = "{ \"jsonRemoved\": \"json\" }";

    private readonly BlacklistSettings _settings;
    
    
    
    public BlacklistService(IOptions<RequestLoggerSettings> settings)
    {
        _settings = settings.Value.BlacklistSettings;
    }
    
    public (bool DoNotStore, Request? request) CheckBlacklist(Request request)
    {
        bool doNotStore = false;
        
        if (_settings.EndpointBlacklist.Any(endpoint => request.Path.StartsWith(endpoint)))
        {
            doNotStore = true;
            // just short circuit the response as nothing else matters
            return (doNotStore, null);
        }
        
        foreach (var header in _settings.HeaderBlacklist.Where(header => request.Headers.ContainsKey(header)))
        {
            request.Headers.Remove(header);
        }

        if (request.QueryParams != null)
        {
            foreach (var queryParam in _settings.QueryParamBlacklist.Where(queryParam => request.QueryParams.ContainsKey(queryParam)))
            {
                request.QueryParams.Remove(queryParam);
            }
        }

        if (_settings.BodyStorageBlacklist.Any(body => request.Path.StartsWith(body)))
        {
            request.Body = BodyBlacklistJson;
        }

        return (doNotStore, request);
    }
}