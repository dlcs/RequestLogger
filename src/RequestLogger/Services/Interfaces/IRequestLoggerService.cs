using RequestLogger.Dto;

namespace RequestLogger.Services.Interfaces;

public interface IRequestLoggerService
{
    Task<Request?> WriteLogMessage(Request request);
}