using System.Text.Json;
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

// Register Serilog
builder.Logging.AddSerilog(logger);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); 
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RequestLoggerSettings>(builder.Configuration.GetSection("RequestLogger"));

builder.Services.AddRequestLoggerContext(builder.Configuration);
builder.Services.AddScoped<IRequestLoggerService, RequestLoggerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    RequestLoggerContextConfiguration.TryRunMigrations(builder.Configuration);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    context.Request.Body.Position = 0;

    var rawRequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    //Console.WriteLine(rawRequestBody);
    
    using var scoped = app.Services.CreateScope();
    var loggerService = scoped.ServiceProvider.GetRequiredService<IRequestLoggerService>();

    var request = new Request()
    {
        Verb = context.Request.Method,
        Service = context.Request.Scheme,
        Customer = context.Request.Headers.Authorization,
        Path = context.Request.Path,
        QueryParams = context.Request.QueryString.ToString(),
        Body = rawRequestBody,
        Headers = JsonSerializer.Serialize(context.Request.Headers),
        RequestTime = DateTime.Now
    };
    
    Console.WriteLine(JsonSerializer.Serialize(request));
    
    await loggerService.WriteLogMessage(request);
    
    
    await context.Response.WriteAsync("Ok");
    return;

    // This is never hit, but the code complains if it's not there
    await next(context);
});

app.MapControllers();

app.Run();
