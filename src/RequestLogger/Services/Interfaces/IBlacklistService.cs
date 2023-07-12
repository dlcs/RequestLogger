using RequestLogger.Dto;

namespace RequestLogger.Services.Interfaces;

public interface IBlacklistService
{
    (bool DoNotStore, Request? request) CheckBlacklist(Request request);
}