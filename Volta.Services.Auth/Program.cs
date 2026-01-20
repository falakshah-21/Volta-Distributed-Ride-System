using Microsoft.EntityFrameworkCore;
using Volta.Services.Auth.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DATABASE CONNECTION ---
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. ENABLE CORS (THE FIX) ---
// This tells the server: "It's okay if the website talks to me."
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   // Allows localhost:5006 (your frontend)
              .AllowAnyMethod()   // Allows POST (Register/Login)
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

// --- 3. ACTIVATE CORS ---
// Important: This must be BEFORE "UseAuthorization"
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();