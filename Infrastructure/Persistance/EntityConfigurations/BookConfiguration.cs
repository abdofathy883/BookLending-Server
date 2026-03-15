using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.EntityConfigurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Status)
                .IsRequired()
                .HasDefaultValue(BookStatus.Free);

            builder.HasMany(b => b.BookBorrows)
                .WithOne(bb => bb.Book)
                .HasForeignKey(bb => bb.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
