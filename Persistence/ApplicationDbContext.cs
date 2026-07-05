using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHttpContextAccessor httpContextAccessor):IdentityDbContext<ApplicationUser,ApplicationRole,string>(options)
    {
        private readonly DbContextOptions<ApplicationDbContext> options = options;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public DbSet<Answer> Answeers { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteAnswer> VoteAnswers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            var cascadeFks= modelBuilder.Model.GetEntityTypes().SelectMany(t=>t.GetForeignKeys())
                .Where(fk=>fk.DeleteBehavior== DeleteBehavior.Cascade&&!fk.IsOwnership);
            foreach(var fk in cascadeFks)
            {
                fk.DeleteBehavior=DeleteBehavior.Restrict;
            }
            base.OnModelCreating(modelBuilder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entires= ChangeTracker.Entries<AuditableEntity>();
            foreach (var entry in entires)
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)!.Value!;
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.CreatedById= userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedOn = DateTime.UtcNow;
                    entry.Entity.UpdatedById = userId;
                }
            }
                    return base.SaveChangesAsync(cancellationToken);
        }
    }
}
