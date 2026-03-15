using Domain.Enums;

namespace Application.DTOs.Books
{
    public class BookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public BookStatus Status { get; set; }
    }
}
