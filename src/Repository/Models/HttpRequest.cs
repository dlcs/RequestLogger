namespace Repository.Models;

public class HttpRequest
{
    /// <summary>
    /// Unique identifier for a request
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The http verb associated with the request
    /// </summary>
    public string Verb { get; set; } = null!;

    /// <summary>
    /// The calling service
    /// </summary>
    public string? Service { get; set; }
    
    /// <summary>
    /// The customer calling the service
    /// </summary>
    public string? Customer { get; set; }

    /// <summary>
    /// The HTTP path of the request
    /// </summary>
    public string Path { get; set; } = null!;
    
    /// <summary>
    /// The query parameters on the request
    /// </summary>
    public string? QueryParams { get; set; }
    
    /// <summary>
    /// The body of the request
    /// </summary>
    public string? Body { get; set; }
    
    /// <summary>
    /// The headers associated with a request
    /// </summary>
    public string? Headers { get; set; }
    
    /// <summary>
    /// The time the request was made
    /// </summary>
    public DateTime RequestTime { get; set; }
}