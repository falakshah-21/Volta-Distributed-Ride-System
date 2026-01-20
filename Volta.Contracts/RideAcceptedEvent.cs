namespace Volta.Contracts
{
    public interface RideAcceptedEvent
    {
        int RideId { get; }
        int DriverId { get; } // Who accepted it?
        DateTime AcceptedAt { get; }
    }
}