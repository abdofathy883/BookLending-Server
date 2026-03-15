using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class OverdueBorrowConfig : IEntityTypeConfiguration<OverdueBorrow>
    {
        public void Configure(EntityTypeBuilder<OverdueBorrow> builder)
        {
            builder.HasKey(bb => bb.Id);

            builder.Property(b => b.BookId)
                .IsRequired();

            builder.Property(bb => bb.BorrowDate)
                .IsRequired();

            builder.Property(bb => bb.ReturnDate)
                .IsRequired(false);

            builder.Property(bb => bb.UserId)
                .IsRequired();

            builder.HasOne(bb => bb.Book)
                .WithMany(b => b.OverdueBorrows)
                .HasForeignKey(bb => bb.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bb => bb.User)
                .WithMany(u => u.OverdueBorrows)
                .HasForeignKey(bb => bb.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
