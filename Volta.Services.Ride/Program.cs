using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Volta.Services.Ride.Data;
using MassTransit;
using Volta.Services.Ride.Controllers;
using Volta.Services.Ride.Hubs;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DATABASE SETUP ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RideDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. SECURITY SETUP (JWT) ---
// Matches the Auth Service configuration for security
var secretKey = "VoltaProjectSecretKey-MustBeVeryLongToWorkSecurely!!";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            // We turn these ON for professional security
            ValidateIssuer = true,
            ValidIssuer = "volta-auth",
            ValidateAudience = true,
            ValidAudience = "volta-users",
            ValidateLifetime = true
        };
    });

// --- 3. RABBITMQ (MassTransit) ---
builder.Services.AddMassTransit(x =>
{
    // Register the Consumer so it can listen to messages
    x.AddConsumer<RideAcceptedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("ride-service-queue", e =>
        {
            e.ConfigureConsumer<RideAcceptedConsumer>(context);
        });
    });
});

// --- 4. SIGNALR & CORS ---
builder.Services.AddSignalR(); // Real-time notifications

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Critical for SignalR
    });
});

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

// --- 5. MIDDLEWARE PIPELINE ---
app.MapHealthChecks("/health");

app.UseCors(); // Must be before Auth

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Endpoint
app.MapHub<NotificationHub>("/notificationHub");

app.Run();