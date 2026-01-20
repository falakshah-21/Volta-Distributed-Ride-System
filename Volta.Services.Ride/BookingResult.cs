namespace Volta.Services.Ride
{
    public class BookingResult
    {
        public required string DriverName { get; set; }
        public required string CarModel { get; set; }
        public required string Eta { get; set; }
        public double Price { get; set; }
    }
}