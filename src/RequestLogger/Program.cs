using System.Text.Json;
using System.Web;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Repository;
using RequestLogger.Dto;
using RequestLogger.Services;
using RequestLogger.Services.Interfaces;
using RequestLogger.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// remove default logging providers
builder.Logging.ClearProviders();
// Serilog configuration        
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger = logger;

builder.Logging.AddSerilog(logger);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RequestLoggerSettings>(builder.Configuration.GetSection("RequestLoggerSettings"));

builder.Services.AddRequestLoggerContext(builder.Configuration);

builder.Services.AddHealthChecks().AddDbContextCheck<RequestLoggerContext>();

builder.Services.AddScoped<IRequestLoggerService, RequestLoggerService>();

builder.Services.AddScoped<IBlacklistService, BlacklistService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Log.Information("Request logger settings: {@Settings}", app.Configuration.GetSection("RequestLoggerSettings").Get<RequestLoggerSettings>());

RequestLoggerContextConfiguration.TryRunMigrations(builder.Configuration);

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

app.Run(async (context) =>
{
    context.Request.EnableBuffering();
    context.Request.Body.Position = 0;

    var requestBody = await GetRequestBody(context);

    using var scoped = app.Services.CreateScope();

    // gets the customer id from a path like somePath/customer/<customer id>/somePath
    var customerId = TryGetCustomerId(context);
    
    var requestLoggerService = scoped.ServiceProvider.GetRequiredService<IRequestLoggerService>();

    // converts query string into a dictionary (if it has values)
    var queryStringDictionary = BuildQueryStringDictionary(context);

    var service = TryGetServiceName(context);
    
    var request = new Request()
    {
        Verb = context.Request.Method,
        Service = service,
        Customer = customerId,
        Path = context.Request.Path,
        QueryParams = queryStringDictionary,
        Body = requestBody,
        Headers = context.Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString()),
        RequestTime = DateTime.UtcNow
    };
    
    var requestCompleted = await requestLoggerService.WriteLogMessage(request);
    
    await SendResponse(requestCompleted, context);
});

async Task SendResponse(Request? request, HttpContext httpContext)
{
    try
    {
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(request));
    }
    catch (Exception exception)
    {
        Log.Error(exception, "Error writing a response");
    }
}

string TryGetServiceName(HttpContext httpContext)
{
    var s = httpContext.Request.Headers.TryGetValue("X-Service", out var header)
        ? header.ToString()
        : httpContext.Request.Host.Value;
    return s;
}

Dictionary<string, string>? BuildQueryStringDictionary(HttpContext httpContext)
{
    var parsedQueryString = HttpUtility.ParseQueryString(httpContext.Request.QueryString.ToString());
    var dictionary = parsedQueryString.HasKeys()
        ? parsedQueryString.AllKeys.ToDictionary(k => k!, k => parsedQueryString[k]!)
        : null;
    return dictionary;
}

string? TryGetCustomerId(HttpContext httpContext)
{
    var s = httpContext.Request.Path.ToString().Split('/')
        .SkipWhile(p => !p.Equals("customers", StringComparison.OrdinalIgnoreCase)).Skip(1).FirstOrDefault();
    return s;
}

async Task<string?> GetRequestBody(HttpContext context)
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

    if (!requestBody.Equals(string.Empty))
    {
        // just convert the body into minimal Json if it isn't Json
        if (!IsJsonValid(requestBody))
        {
            requestBody = $"{{ \"invalidJson\": \"{requestBody}\" }}";
        }
    }
    else
    {
        requestBody = null;
    }

    return requestBody;
}

bool IsJsonValid(string json)
{
    if (string.IsNullOrWhiteSpace(json))
        return false;

    try
    {
        using var jsonDoc = JsonDocument.Parse(json);
        return true;
    }
    catch (JsonException)
    {
        return false;
    }
}

app.Run();

public partial class Program { }
