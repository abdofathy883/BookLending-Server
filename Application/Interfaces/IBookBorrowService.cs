using Application.DTOs.Books;

namespace Application.Interfaces
{
    public interface IBookBorrowService
    {
        Task<List<BookBorrowDto>> GetAll();
        Task<List<OverdueBorrowDto>> GetAllOverdue();
        Task<BookBorrowDto> BorrowBook(BorrowBookDto borrowDto, string userId);
        Task<BookBorrowDto> ReturnBook(ReturnBookDto returnDto, string userId);
        Task<List<BookBorrowDto>> GetByBookId(int bookId);
        Task<List<OverdueBorrowDto>> MarkBooksAsOverdue();
    }
}
