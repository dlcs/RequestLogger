using Repository;
using RequestLogger.Dto;
using RequestLogger.Dto.Converters;
using RequestLogger.Services.Interfaces;

namespace RequestLogger.Services;

public class RequestLoggerService : IRequestLoggerService
{
    private readonly RequestLoggerContext _context;
    private readonly IBlacklistService _blacklistService;
    public RequestLoggerService(RequestLoggerContext context, IBlacklistService blacklistService)
    {
        _context = context;
        _blacklistService = blacklistService;
    }
    
    public async Task<Request?> WriteLogMessage(Request request)
    {
        var (doNotStore, requestAfterBlacklist) = _blacklistService.CheckBlacklist(request);

        if (doNotStore) return requestAfterBlacklist;

        if (requestAfterBlacklist != null)
        {
            var httpRequest = RequestConverter.ConvertRequest(requestAfterBlacklist);
            await _context.Requests.AddAsync(httpRequest);
        }

        await _context.SaveChangesAsync();

        return requestAfterBlacklist;
    }
}