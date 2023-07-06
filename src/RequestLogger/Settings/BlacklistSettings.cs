namespace RequestLogger.Settings;

public class BlacklistSettings
{
    public List<string> HeaderBlacklist { get; init; } = new();

    public List<string> QueryParamBlacklist { get; init; } = new();

    public List<string> EndpointBlacklist { get; init; } = new();

    public List<string> BodyStorageBlacklist { get; set; } = new();
}