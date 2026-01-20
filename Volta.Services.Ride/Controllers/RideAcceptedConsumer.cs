using MassTransit;
using Volta.Contracts;
using Volta.Services.Ride.Data;
using Microsoft.AspNetCore.SignalR; // NEEDED: For Real-Time
using Volta.Services.Ride.Hubs;     // NEEDED: To find the Hub class

namespace Volta.Services.Ride.Controllers
{
    // Listens for "RideAcceptedEvent"
    public class RideAcceptedConsumer : IConsumer<RideAcceptedEvent>
    {
        private readonly RideDbContext _context;
        private readonly ILogger<RideAcceptedConsumer> _logger;
        private readonly IHubContext<NotificationHub> _hubContext; // 1. NEW: The Broadcast Tool

        // Inject Database AND SignalR Hub
        public RideAcceptedConsumer(RideDbContext context,
                                    ILogger<RideAcceptedConsumer> logger,
                                    IHubContext<NotificationHub> hubContext) // 2. NEW: Inject it here
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<RideAcceptedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("📢 RIDE ACCEPTED! Updating Ride {RideId}...", message.RideId);

            // 1. Find the Ride in SQL Database
            var ride = await _context.Rides.FindAsync(message.RideId);

            if (ride != null)
            {
                // 2. Update the Status
                ride.Status = "OnRoute";

                // 3. Save Changes
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Database Updated: Ride {RideId} is now OnRoute.", message.RideId);

                // 4. NEW: Broadcast to the User's Phone! 📲
                // This sends a message to the JavaScript code in index.html
                await _hubContext.Clients.All.SendAsync("ReceiveRideUpdate",
                    $"Driver #{message.DriverId} has accepted your ride! Status: OnRoute.");
            }
            else
            {
                _logger.LogError("❌ Error: Could not find Ride {RideId} in database.", message.RideId);
            }
        }
    }
}