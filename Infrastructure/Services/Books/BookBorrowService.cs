using Application.DTOs.Books;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Books
{
    public class BookBorrowService : IBookBorrowService
    {
        private readonly BookLendingDbContext context;
        private readonly IMapper mapper;

        public BookBorrowService(BookLendingDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task<BookBorrowDto> BorrowBook(BorrowBookDto borrowDto, string userId)
        {
            var book = await context.Books.FindAsync(borrowDto.BookId)
                ?? throw new KeyNotFoundException("الكتاب غير موجود");

            if (book.Status == BookStatus.Borrowed)
                throw new InvalidOperationException("الكتاب مستعار بالفعل");

            if (book.Status == BookStatus.Lost)
                throw new InvalidOperationException("الكتاب مفقود ولا يمكن استعارته");

            book.Status = BookStatus.Borrowed;

            var bookBorrow = new BookBorrow
            {
                BookId = book.Id,
                Book = book,
                UserId = userId,
                BorrowDate = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(7),
                IsReturned = false
            };

            await context.BookBorrows.AddAsync(bookBorrow);
            await context.SaveChangesAsync();

            return mapper.Map<BookBorrowDto>(bookBorrow);
        }

        public async Task<BookBorrowDto> ReturnBook(ReturnBookDto returnDto, string userId)
        {
            var book = await context.Books
                .FindAsync(returnDto.BookId)
                ?? throw new KeyNotFoundException("الكتاب غير موجود");

            if (book.Status != BookStatus.Borrowed)
                throw new InvalidOperationException("الكتاب غير مستعار حالياً");

            var activeBorrow = await context.BookBorrows
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .Where(bb => bb.BookId == returnDto.BookId && bb.UserId == userId)
                .OrderByDescending(bb => bb.BorrowDate)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("لا يوجد سجل استعارة نشط لهذا الكتاب");

            activeBorrow.IsReturned = true;
            book.Status = BookStatus.Free;

            await context.SaveChangesAsync();
            return mapper.Map<BookBorrowDto>(activeBorrow);
        }

        public async Task<List<BookBorrowDto>> GetAll()
        {
            var borrows = await context.BookBorrows
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .OrderByDescending(bb => bb.BorrowDate)
                .ToListAsync();

            return mapper.Map<List<BookBorrowDto>>(borrows);
        }
        
        public async Task<List<OverdueBorrowDto>> GetAllOverdue()
        {
            var overdues = await context.OverdueBorrows
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .OrderByDescending(bb => bb.BorrowDate)
                .ToListAsync();

            return mapper.Map<List<OverdueBorrowDto>>(overdues);
        }

        public async Task<List<BookBorrowDto>> GetByBookId(int bookId)
        {
            var bookExists = await context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                throw new KeyNotFoundException("الكتاب غير موجود");

            var borrows = await context.BookBorrows
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .Where(bb => bb.BookId == bookId)
                .OrderByDescending(bb => bb.BorrowDate)
                .ToListAsync();

            return mapper.Map<List<BookBorrowDto>>(borrows);
        }

        public async Task<List<OverdueBorrowDto>> MarkBooksAsOverdue()
        {
            DateTime today = DateTime.UtcNow.Date;
            var borrows = await context.BookBorrows
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .Where(bb => !bb.IsReturned && bb.ReturnDate < today)
                .OrderByDescending(bb => bb.BorrowDate)
                .ToListAsync();

            foreach (var borrow in borrows)
            {
                bool alreadyLogged = await context.OverdueBorrows
                    .AnyAsync(o => o.BookId == borrow.BookId && o.BorrowDate == borrow.BorrowDate);

                if (!alreadyLogged)
                {
                    var overdueBorrow = new OverdueBorrow
                    {
                        UserId = borrow.UserId,
                        BookId = borrow.BookId,
                        BorrowDate = borrow.BorrowDate,
                        ReturnDate = borrow.ReturnDate
                    };
                    await context.OverdueBorrows.AddAsync(overdueBorrow);
                }
            }
            await context.SaveChangesAsync();
            return mapper.Map<List<OverdueBorrowDto>>(borrows);
        }
    }
}
