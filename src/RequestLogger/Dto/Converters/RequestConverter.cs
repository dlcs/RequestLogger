using System.Text.Json;
using HttpRequest = Repository.Models.HttpRequest;

namespace RequestLogger.Dto.Converters;

public static class RequestConverter
{
    public static HttpRequest ConvertRequest(Request request)
    {
        return new HttpRequest
        {
            Verb = request.Verb,
            Service = request.Service,
            Customer = request.Customer,
            Path = request.Path,
            QueryParams = request.QueryParams != null ? JsonSerializer.Serialize(request.QueryParams) : null,
            Body = request.Body,
            Headers = JsonSerializer.Serialize(request.Headers),
            RequestTime = request.RequestTime
        };
    }
}