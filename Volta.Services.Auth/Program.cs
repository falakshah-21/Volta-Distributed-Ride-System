using Microsoft.EntityFrameworkCore;
using Volta.Services.Auth.Data;
using Volta.Services.Auth.Entities; // Yeh line add ki hai Entity ke liye

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

// =======================================================
// 🌟 PROFESSIONAL DATABASE SEEDING (AUTO-ADMIN CREATION)
// =======================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AuthDbContext>();

        // Check karte hain ke kya database mein koi Admin already majood hai?
        if (!context.Users.Any(u => u.Role == "Admin"))
        {
            var defaultAdmin = new User
            {
                Username = "superadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), 
                Role = "Admin",
                IsApproved = true
            };

            context.Users.Add(defaultAdmin);
            context.SaveChanges();
            
            Console.WriteLine("Default Admin User created successfully! (admin@volta.com / Admin@123)");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}
// =======================================================

app.MapHealthChecks("/health");

// --- 3. ACTIVATE CORS ---
// Important: This must be BEFORE "UseAuthorization"
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();