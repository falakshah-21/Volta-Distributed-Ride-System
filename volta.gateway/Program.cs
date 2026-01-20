using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client; // <--- 1. NEW NAMESPACE

var builder = WebApplication.CreateBuilder(args);

// --- 1. SECURITY: CORS POLICY ---
string[] allowedOrigins =
{
    "http://localhost:5006",
    "http://FALAK:5006",
    "http://192.168.3.110:5006", // Your IP
    "http://0.0.0.0:5006"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("VoltaAppOnly",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

// --- 2. CONFIGURATION ---
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// --- 3. HEALTH CHECKS & MONITORING ---
builder.Services.AddOcelot(builder.Configuration);

// Add basic Health Checks
builder.Services.AddHealthChecks();

// 🔥 ADD DASHBOARD SERVICES (Stores history in memory)
builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetEvaluationTimeInSeconds(5); // Check every 5 seconds
    setup.MaximumHistoryEntriesPerEndpoint(60);
}).AddInMemoryStorage();

var app = builder.Build();

// --- 4. PIPELINE ---
app.UseCors("VoltaAppOnly");

// 🔥 CONFIGURE HEALTH ENDPOINTS
// A. Data Endpoint (The Dashboard reads this)
app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// B. Visual Dashboard (You read this)
app.UseHealthChecksUI(config =>
{
    config.UIPath = "/dashboard"; // Access at http://localhost:5000/dashboard
});

// Start the Gateway
await app.UseOcelot();

app.Run();