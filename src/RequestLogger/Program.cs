using System.Text.Json;
using System.Web;
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

// Register Serilog
builder.Logging.AddSerilog(logger);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); 
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RequestLoggerSettings>(builder.Configuration.GetSection("RequestLoggerSettings"));

builder.Services.AddRequestLoggerContext(builder.Configuration);
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run(async (context) =>
{
    context.Request.EnableBuffering();
    context.Request.Body.Position = 0;

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

    using var scoped = app.Services.CreateScope();
    var loggerService = scoped.ServiceProvider.GetRequiredService<IRequestLoggerService>();

    // gets the customer id from a path like somePath/customer/<customer id>/somePath
    var customerId = context.Request.Path.ToString().Split('/')
        .SkipWhile(p => !p.Equals("customer", StringComparison.OrdinalIgnoreCase)).Skip(1).FirstOrDefault();

    // converts query string into a dictionary (if it has values)
    var parsedQueryString = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
    var queryStringDictionary = parsedQueryString.HasKeys() ? parsedQueryString.AllKeys.ToDictionary(k => k!, k => parsedQueryString[k]!) : null;

    var service = context.Request.Headers.TryGetValue("X-Service", out var header) ? header.ToString() : context.Request.Host.Value;
    
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
    
    var requestCompleted = await loggerService.WriteLogMessage(request);
    
    await context.Response.WriteAsync(JsonSerializer.Serialize(requestCompleted));
});

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
