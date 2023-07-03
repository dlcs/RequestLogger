using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// remove default logging providers
builder.Logging.ClearProviders();
// Serilog configuration        
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// Register Serilog
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    context.Request.Body.Position = 0;

    var rawRequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    Console.WriteLine(rawRequestBody);

    await context.Response.WriteAsync("Ok");
    return;

    // This is never hit, but the code complains if it's not there
    await next(context);
});

app.MapControllers();

app.Run();
