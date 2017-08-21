using Microsoft.EntityFrameworkCore;

namespace Surpass.Web.Data
{
    public class SurpassWebContext : DbContext
    {
        public SurpassWebContext (DbContextOptions<SurpassWebContext> options)
            : base(options)
        {
        }

        public DbSet<Surpass.Entities.User> User { get; set; }
    }
}
