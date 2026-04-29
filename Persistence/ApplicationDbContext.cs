using Microsoft.EntityFrameworkCore;
using SurveyBasket.Persistence.EntitiesConfiguration;
using System.Reflection;

namespace SurveyBasket.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options)
    {
        public DbSet<Poll> Polls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
