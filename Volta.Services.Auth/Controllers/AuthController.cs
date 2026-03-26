using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Volta.Services.Auth.Data;
using Volta.Services.Auth.Entities;
// Note: System.Security.Cryptography hata diya gaya hai kyunke ab hum BCrypt use karenge.

namespace Volta.Services.Auth.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private const string SecretKey = "VoltaProjectSecretKey-MustBeVeryLongToWorkSecurely!!";

        public AuthController(AuthDbContext context)
        {
            _context = context;
        }

        // --- 1. REGISTER (Updated with BCrypt & Gatekeeper Logic) ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            // A. Check if Username Exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username is already taken.");

            // B. Prevent Public Admin Registration
            if (request.Role == "Admin" || request.Role == "SuperAdmin")
                return BadRequest("Admins cannot register publicly.");

            // --- C. GATEKEEPER LOGIC ---
            // Passengers are approved immediately. Drivers must wait for Admin.
            bool isApproved = request.Role == "Passenger";

            // D. Create User Object (Using BCrypt for Password Hashing)
            var newUser = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                // PROFESSIONAL HASHING:
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), 
                Role = request.Role,
                IsApproved = isApproved, // <--- SAVING THE STATUS HERE

                // Conditional Logic: Only Drivers get Car Details
                CarName = request.Role == "Driver" ? request.CarName : null,
                CarModel = request.Role == "Driver" ? request.CarModel : null,
                CarNumber = request.Role == "Driver" ? request.CarNumber : null
            };

            // E. Save to SQL
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // F. Return different messages based on role
            if (!isApproved)
            {
                return Ok(new { message = "Registration Successful! Please wait for Admin approval before logging in." });
            }

            return Ok(new { message = "Registration Successful!" });
        }

        // --- 2. LOGIN (Updated with BCrypt & Approval Check) ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            // A. Find User by Username ONLY
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid Username or Password" });

            // --- B. VERIFY PASSWORD USING BCRYPT ---
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
                return Unauthorized(new { message = "Invalid Username or Password" });

            // --- C. CHECK APPROVAL STATUS ---
            if (!user.IsApproved)
            {
                return Unauthorized(new { message = "Account Pending! An Admin must approve your driver account first." });
            }

            // D. Generate Token
            var token = GenerateJwtToken(user);
            return Ok(new { token, role = user.Role });
        }

        // --- HELPER FUNCTIONS ---
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "volta-auth",
                audience: "volta-users",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

    // DTOs
    public class LoginDto { public string Username { get; set; } public string Password { get; set; } }

    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string? CarName { get; set; }
        public string? CarModel { get; set; }
        public string? CarNumber { get; set; }
    }
}