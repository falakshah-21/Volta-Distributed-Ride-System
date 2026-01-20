using System.ComponentModel.DataAnnotations;

namespace Volta.Services.Auth.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        // --- COMMON FIELDS (For Everyone) ---
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Roles: "Passenger", "Driver", "Admin", "SuperAdmin"
        public string Role { get; set; } = "Passenger";

        // --- NEW: Security Flag (Gatekeeper) ---
        // True = Can Log in. False = Blocked (Wait for Admin).
        public bool IsApproved { get; set; }

        // --- DRIVER SPECIFIC FIELDS (Optional/Nullable) ---
        public string? CarName { get; set; }
        public string? CarModel { get; set; }
        public string? CarNumber { get; set; }
    }
}