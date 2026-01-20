using Microsoft.EntityFrameworkCore;
using Volta.Services.Driver.Data;
using MassTransit;                      // NEEDED: To talk to RabbitMQ
using Volta.Services.Driver.Controllers; // NEEDED: To find the Consumer class

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Add Database
builder.Services.AddDbContext<DriverDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Add RabbitMQ (MassTransit) - THE NEW PART 🐇
builder.Services.AddMassTransit(x =>
{
    // A. Register the Consumer (The Listener)
    x.AddConsumer<RideRequestedConsumer>();

    // B. Configure the Connection
    x.UsingRabbitMq((context, cfg) =>
    {
        // Connect to Docker RabbitMQ
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // C. Setup the Inbox (Queue)
        // This tells RabbitMQ: "Create a queue named 'driver-service-queue' and put messages for me there."
        cfg.ReceiveEndpoint("driver-service-queue", e =>
        {
            e.ConfigureConsumer<RideRequestedConsumer>(context);
        });
    });
});

builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapHealthChecks("/health");

// 4. Configure Pipeline
app.UseAuthorization();

app.MapControllers();

// 5. Run the app
app.Run();