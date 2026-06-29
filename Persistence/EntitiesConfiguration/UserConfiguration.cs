

using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Persistence.EntitiesConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            
            builder.Property(x=>x.FirstName).HasMaxLength(100);
            builder.Property(x=>x.LastName).HasMaxLength(100);
            builder.OwnsMany(u => u.RefreshTokens).ToTable("RefreshTokens").WithOwner()
                .HasForeignKey("UserId");

            var passowrdHasher = new PasswordHasher<ApplicationUser>();
            builder.HasData(new ApplicationUser
            {
                Id = DefaultUsers.AdminId,
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                EmailConfirmed = true,
                PasswordHash = DefaultUsers.AdminPassword,
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                FirstName = "SurveyBasket",
                LastName = "Admin"
            });
        }
    }   
}
