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
            QueryParams = request.QueryParams,
            Body = request.Body,
            Headers = request.Headers,
            RequestTime = request.RequestTime
        };
    }
}