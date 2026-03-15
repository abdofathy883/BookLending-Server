using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class BookBorrowConfiguration : IEntityTypeConfiguration<BookBorrow>
    {
        public void Configure(EntityTypeBuilder<BookBorrow> builder)
        {
            builder.HasKey(bb => bb.Id);

            builder.Property(b => b.BookId)
                .IsRequired();

            builder.Property(bb => bb.BorrowDate)
                .IsRequired();

            builder.Property(bb => bb.ReturnDate)
                .IsRequired(false);

            builder.HasOne(bb => bb.Book)
                .WithMany(b => b.BookBorrows)
                .HasForeignKey(bb => bb.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
