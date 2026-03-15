using Domain.Enums;

namespace Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public BookStatus Status { get; set; }
        public List<BookBorrow> BookBorrows { get; set; } = new();
        public List<OverdueBorrow> OverdueBorrows { get; set; } = new();
    }
}
