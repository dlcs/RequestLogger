namespace RequestLogger.Settings;

public class BlacklistSettings
{
    /// <summary>
    /// Header blacklist
    /// </summary>
    public List<string> HeaderBlacklist { get; init; } = new();

    /// <summary>
    /// Query parameter blacklist
    /// </summary>
    public List<string> QueryParamBlacklist { get; init; } = new();

    /// <summary>
    /// Endpoint blacklist
    /// </summary>
    public List<string> EndpointBlacklist { get; init; } = new();

    /// <summary>
    /// Request body blacklist
    /// </summary>
    public List<string> BodyStorageBlacklist { get; set; } = new();
}