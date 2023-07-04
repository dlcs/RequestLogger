using Repository;
using RequestLogger.Dto;
using RequestLogger.Dto.Converters;
using RequestLogger.Services.Interfaces;

namespace RequestLogger.Services;

public class RequestLoggerService : IRequestLoggerService
{
    private RequestLoggerContext _context;
    public RequestLoggerService(RequestLoggerContext context)
    {
        _context = context;
    }
    
    public async Task WriteLogMessage(Request request)
    {
        var httpRequest = RequestConverter.ConvertRequest(request);
        await _context.Requests.AddAsync(httpRequest);

        await _context.SaveChangesAsync();
    }
}