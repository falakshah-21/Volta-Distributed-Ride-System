namespace Volta.Contracts
{
    public interface RideRequestedEvent
    {
        int RideId { get; }
        string PickupLocation { get; }
        string DropoffLocation { get; }
        decimal Price { get; }
    }
}