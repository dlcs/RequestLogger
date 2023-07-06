using System.Collections.Specialized;

namespace RequestLogger.Dto;

public class Request
{
    /// <summary>
    /// The http verb associated with the request
    /// </summary>
    public string Verb { get; init; } = null!;

    /// <summary>
    /// The calling service
    /// </summary>
    public string? Service { get; init; }
    
    /// <summary>
    /// The customer calling the service
    /// </summary>
    public string? Customer { get; init; }

    /// <summary>
    /// The HTTP path of the request
    /// </summary>
    public string Path { get; init; } = null!;
    
    /// <summary>
    /// The query parameters on the request
    /// </summary>
    public Dictionary<string, string>? QueryParams { get; init; }
    
    /// <summary>
    /// The body of the request
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// The headers associated with a request
    /// </summary>
    public Dictionary<string, string> Headers { get; init; } = null!;
    
    /// <summary>
    /// The time the request was made
    /// </summary>
    public DateTime RequestTime { get; init; }
}