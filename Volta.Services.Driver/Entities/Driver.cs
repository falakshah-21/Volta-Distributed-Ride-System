namespace Volta.Services.Driver.Entities
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty; // e.g., "Toyota Corolla"
        public string LicensePlate { get; set; } = string.Empty; // e.g., "KHI-1234"
        public string Status { get; set; } = "Offline"; // "Online", "Busy", "Offline"
    }
}