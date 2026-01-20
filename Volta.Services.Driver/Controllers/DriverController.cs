using Microsoft.AspNetCore.Mvc;
using Volta.Services.Driver.Data;
using Volta.Services.Driver.Entities;
using MassTransit;                   // NEEDED: To send messages
using Volta.Contracts;               // NEEDED: To format the message

namespace Volta.Services.Driver.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly DriverDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint; // 1. The new Antenna

        // Constructor: Now receives BOTH Database AND RabbitMQ connections
        public DriverController(DriverDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        // --- EXISTING METHOD (Unchanged) ---
        [HttpPost("register")]
        public IActionResult Register([FromBody] Entities.Driver driver)
        {
            _context.Drivers.Add(driver);
            _context.SaveChanges();
            return Ok(driver);
        }

        // --- NEW METHOD (The "Accept" Button) ---
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptRide([FromBody] AcceptRideRequest request)
        {
            // 1. (Optional) In a real app, check if DriverId exists in _context here.

            // 2. Broadcast "I Took It!" to the whole system
            await _publishEndpoint.Publish<RideAcceptedEvent>(new
            {
                RideId = request.RideId,
                DriverId = request.DriverId,
                AcceptedAt = DateTime.UtcNow
            });

            return Ok(new { Status = "Ride Accepted. Notification sent to Server." });
        }
    }

    // Helper class for the data coming in from the driver's phone
    public class AcceptRideRequest
    {
        public int RideId { get; set; }
        public int DriverId { get; set; }
    }
}