using OpenSch.Infrastructure.Data;
using OpenSch.Web.Infrastructure;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

// CORS: only the frontend origins listed in config may call the API.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?.Select(origin => origin.Trim())
    .Where(origin => origin.Length > 0)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray()
    ?? ["http://localhost:5173"];

// Guard: AllowCredentials + wildcard is rejected by the browser and by ASP.NET Core.
// Also reject partial wildcards so a typo such as "https://*.example.com" fails fast.
if (allowedOrigins.Length == 0)
    throw new InvalidOperationException("Cors:AllowedOrigins must contain at least one explicit origin.");

if (allowedOrigins.Any(origin => origin.Contains('*', StringComparison.Ordinal)))
    throw new InvalidOperationException(
        "Wildcard origins cannot be used in Cors:AllowedOrigins when AllowCredentials is set. " +
        "Specify explicit origins instead.");

builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()));

// Rate limiting: the AI endpoint is capped per authenticated user to prevent cost abuse.
// AddPolicy creates one limiter per partition; AddSlidingWindowLimiter would share a
// single counter across every caller.
builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opts.AddPolicy("ai-per-user", httpContext =>
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var partitionKey = string.IsNullOrWhiteSpace(userId)
            ? $"ip:{httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}"
            : $"user:{userId}";

        return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => new SlidingWindowRateLimiterOptions
        {
            PermitLimit          = 10,
            Window               = TimeSpan.FromMinutes(1),
            SegmentsPerWindow    = 2,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit           = 0,
        });
    });
});

var app = builder.Build();

// The OpenAPI document generator (GetDocument.Insider) boots this assembly at build time
// with no configuration, so migrations must not run inside that process.
var isOpenApiDocumentGeneration =
    System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";

// Apply migrations and seed reference data on startup in every environment so a fresh
// deployment never starts against an unmigrated or empty database.
if (!isOpenApiDocumentGeneration)
{
    await app.InitialiseDatabaseAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHsts();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("Frontend");
// Registration is bot-protected before the Identity endpoint runs.
app.UseMiddleware<TurnstileMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
// Rate limiting runs after authentication so the AI policy can partition by user.
app.UseRateLimiter();
app.UseFileServer();

app.MapDefaultEndpoints();
if (!app.Environment.IsDevelopment())
{
    // Container platforms (Railway, App Service, ACA) need a cheap liveness endpoint.
    // Startup already ran the migrations, so a 200 also means the database is reachable.
    app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
}
app.MapEndpoints(typeof(Program).Assembly);
app.MapFallbackToFile("index.html");

app.Run();
