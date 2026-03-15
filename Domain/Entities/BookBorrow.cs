namespace Domain.Entities
{
    public class BookBorrow
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = default!;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }
}
