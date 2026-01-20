using Microsoft.EntityFrameworkCore;
using Volta.Services.Driver.Entities;

namespace Volta.Services.Driver.Data
{
    public class DriverDbContext : DbContext
    {
        public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
        {
        }

        public DbSet<Entities.Driver> Drivers { get; set; }
    }
}