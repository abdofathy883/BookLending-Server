using Application.DTOs.Books;

namespace Application.Interfaces
{
    public interface IBookBorrowService
    {
        Task<BookBorrowDto> BorrowBook(BorrowBookDto borrowDto);
        Task<BookBorrowDto> ReturnBook(ReturnBookDto returnDto);
        Task<List<BookBorrowDto>> GetAll();
        Task<List<BookBorrowDto>> GetByBookId(int bookId);
        Task<List<OverdueBorrowDto>> GetDailyDelayed();
    }
}
