using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class BookLendingDbContext : IdentityDbContext<ApplicationUser>
    {
        public BookLendingDbContext(DbContextOptions<BookLendingDbContext> options) : base(options)
        { }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookBorrow> BookBorrows { get; set; }
        public DbSet<OverdueBorrow> OverdueBorrows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookLendingDbContext).Assembly);
        }
    }
}
