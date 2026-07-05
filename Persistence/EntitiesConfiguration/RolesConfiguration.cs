namespace SurveyBasket.Persistence.EntitiesConfiguration;

public class RolesConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
            new ApplicationRole
            {
                Id = DefaultRoles.Admin.Id,
                Name = DefaultRoles.Admin.Nanme,
                NormalizedName = DefaultRoles.Admin.Nanme.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp
            },
             new ApplicationRole
             {
                 Id = DefaultRoles.Member.Id,
                 Name = DefaultRoles.Member.Name,
                 NormalizedName = DefaultRoles.Member.Name.ToUpper(),
                 ConcurrencyStamp = DefaultRoles.Member.ConcurrencyStamp,
                 IsDefault = true
             }
            );
           
    }
}
