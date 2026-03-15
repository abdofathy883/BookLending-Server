namespace Application.DTOs.Books
{
    public class OverdueBorrowDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
