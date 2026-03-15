using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class AppUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.OwnsMany(u => u.RefreshTokens, rt =>
            {
                rt.WithOwner();
                rt.Property(r => r.Token);
                rt.Property(r => r.ExpiresOn);
                rt.Property(r => r.CreateOn);
            });
        }
    }
}
