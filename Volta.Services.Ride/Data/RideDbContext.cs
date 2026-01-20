using Microsoft.EntityFrameworkCore;
using Volta.Services.Ride.Entities;

namespace Volta.Services.Ride.Data
{
    public class RideDbContext : DbContext
    {
        public RideDbContext(DbContextOptions<RideDbContext> options) : base(options)
        {
        }

        // We use "Entities.Ride" to avoid the error
        public DbSet<Entities.Ride> Rides { get; set; }
    }
}