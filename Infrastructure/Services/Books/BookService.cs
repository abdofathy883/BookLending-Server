using Application.DTOs.Books;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Books
{
    public class BookService : IBookService
    {
        private readonly BookLendingDbContext context;
        private readonly IMapper mapper;

        public BookService(BookLendingDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task<BookDto> Create(CreateBookDto newBook)
        {
            var book = mapper.Map<Book>(newBook);
            book.Status = BookStatus.Free;

            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            return mapper.Map<BookDto>(book);
        }

        public async Task<bool> Delet(int bookId)
        {
            var book = await context.Books.SingleAsync(x => x.Id == bookId);
            context.Books.Remove(book);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<BookDto>> GetAll()
        {
            var books = await context.Books.ToListAsync();
            return mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetById(int id)
        {
            var book = await context.Books.FindAsync(id)
                ?? throw new KeyNotFoundException("الكتاب غير موجود");

            return mapper.Map<BookDto>(book);
        }
    }
}
