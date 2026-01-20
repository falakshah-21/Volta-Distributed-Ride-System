namespace Volta.Services.Ride.Entities
{
    public class RideDto
    {
        public string PassengerName { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public decimal Price { get; set; }
    }
}