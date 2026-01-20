using Microsoft.EntityFrameworkCore;
using Volta.Services.Auth.Entities;

namespace Volta.Services.Auth.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}