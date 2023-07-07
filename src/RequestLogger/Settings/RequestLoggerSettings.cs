namespace RequestLogger.Settings;

public class RequestLoggerSettings
{
    /// <summary>
    /// Settings related to configuring blacklists
    /// </summary>
    public BlacklistSettings BlacklistSettings { get; set; } = new();
}