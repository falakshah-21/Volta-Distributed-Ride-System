namespace Volta.Services.Ride.Entities
{
    public class Ride
    {
        public int Id { get; set; }

        // --- NEW: Random Code for display (e.g., "TX-8821") ---
        public string RideCode { get; set; }

        // --- NEW: Unique IDs (The "Real" Identity) ---
        // We still keep Names for easy display, but logic should use IDs
        public string PassengerId { get; set; }
        public string PassengerName { get; set; }

        public string? DriverId { get; set; }
        public string? DriverName { get; set; }

        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}