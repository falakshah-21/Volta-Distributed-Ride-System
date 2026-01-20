using MassTransit;
using Volta.Contracts;

namespace Volta.Services.Driver.Controllers
{
    // This class listens for the specific "RideRequestedEvent" message
    public class RideRequestedConsumer : IConsumer<RideRequestedEvent>
    {
        private readonly ILogger<RideRequestedConsumer> _logger;

        public RideRequestedConsumer(ILogger<RideRequestedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RideRequestedEvent> context)
        {
            var message = context.Message;

            // Log it to the console so we can see it working
            _logger.LogInformation("✅ DRIVER SERVICE RECEIVED RIDE: {RideId} for {Price}",
                message.RideId, message.Price);

            // In the future, this is where we would:
            // 1. Check which drivers are nearby
            // 2. Send a notification to their phones

            return Task.CompletedTask;
        }
    }
}