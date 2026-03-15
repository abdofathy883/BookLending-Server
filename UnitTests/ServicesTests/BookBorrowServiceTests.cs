using Application.DTOs.Books;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistance;
using Infrastructure.Services.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UnitTests.ServicesTests
{
    public class BookBorrowServiceTests
    {
        private BookLendingDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<BookLendingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new BookLendingDbContext(options);
        }

        private IMapper CreateMapper()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<BookBorrowProfile>();
            });

            return services.BuildServiceProvider().GetRequiredService<IMapper>();
        }

        [Fact]
        public async Task BorrowBook_ShouldReturnDto_WhenBookIsFree()
        {
            // Arrange
            var context = CreateContext();

            var user = new ApplicationUser { Id = "user-1", FullName = "Test User", UserName = "testuser" };
            context.Users.Add(user);


            var book = new Book { Id = 1, Status = BookStatus.Free, Title = "Test Book" };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookBorrowService(context, CreateMapper());

            // Act
            var result = await service.BorrowBook(new BorrowBookDto { BookId = 1 }, "user-1");

            // Assert
            result.Should().NotBeNull();
            result.BookId.Should().Be(1);
        }

        [Fact]
        public async Task BorrowBook_ShouldThrow_WhenBookAlreadyBorrowed()
        {
            // Arrange
            var context = CreateContext();
            context.Books.Add(new Book { Id = 1, Title = "Test Book", Status = BookStatus.Borrowed });
            await context.SaveChangesAsync();

            var service = new BookBorrowService(context, CreateMapper());

            // Act
            var act = async () => await service.BorrowBook(new BorrowBookDto { BookId = 1 }, "user-1");

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
