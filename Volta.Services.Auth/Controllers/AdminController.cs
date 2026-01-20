using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Volta.Services.Auth.Data;
using Volta.Services.Auth.Entities;

namespace Volta.Services.Auth.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public AdminController(AuthDbContext context)
        {
            _context = context;
        }

        // --- 1. GET PENDING DRIVERS ---
        [HttpGet("pending-drivers")]
        public async Task<IActionResult> GetPendingDrivers()
        {
            var drivers = await _context.Users
                .Where(u => u.Role == "Driver" && u.IsApproved == false)
                .Select(u => new { u.Id, u.FullName, u.Username, u.CarName, u.CarNumber })
                .ToListAsync();

            return Ok(drivers);
        }

        // --- 2. APPROVE DRIVER ---
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveDriver(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");

            user.IsApproved = true;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Driver {user.Username} approved!" });
        }

        // --- 3. GET ALL ADMINS (New) ---
        [HttpGet("list-admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _context.Users
                .Where(u => u.Role == "Admin")
                .Select(u => new { u.Id, u.FullName, u.Username })
                .ToListAsync();
            return Ok(admins);
        }

        // --- 4. CREATE NEW ADMIN (New) ---
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username taken.");

            var newAdmin = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                PasswordHash = ComputeSha256Hash(request.Password),
                Role = "Admin",      // Force Role to Admin
                IsApproved = true    // Auto-approve Admins
            };

            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "New Admin Created Successfully!" });
        }

        // --- 5. DELETE USER (New) ---
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User deleted." });
        }

        // Helper for Hashing (Same as AuthController)
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) { builder.Append(bytes[i].ToString("x2")); }
                return builder.ToString();
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Where(u => u.Role == "Passenger" || u.Role == "Driver") // Only regular users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Username,
                    u.Role,
                    u.CarName,
                    u.CarNumber
                })
                .ToListAsync();

            return Ok(users);
        }

    }
}