namespace Domain.Entities
{
    public class OverdueBorrow
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = default!;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
