using Application.DTOs.Books;

namespace Application.Interfaces
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAll();
        Task<BookDto> GetById(int id);
        Task<BookDto> Create(CreateBookDto newBook);
        Task<bool> Delet(int bookId);
    }
}
