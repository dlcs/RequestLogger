using RequestLogger.Dto;

namespace RequestLogger.Services.Interfaces;

public interface IRequestLoggerService
{
    Task WriteLogMessage(Request request);
}