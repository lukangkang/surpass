using Microsoft.EntityFrameworkCore;
using Surpass.Domain.Entities;

namespace Surpass.Infrastructure.Database
{
    public class EFCoreDbContext : DbContext
    {
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<User> User { get; set; }
    }
}
