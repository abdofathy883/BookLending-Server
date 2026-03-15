using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Books
{
    public class OverdueBooksJob
    {
        private readonly IBookBorrowService bookBorrowService;

        public OverdueBooksJob(IBookBorrowService bookBorrowService)
        {
            this.bookBorrowService = bookBorrowService;
        }

        public async Task CheckOverdueBooksAsync()
        {
            var overdueBooks = await bookBorrowService.MarkBooksAsOverdue();

            if (!overdueBooks.Any())
            {
                return;
            }

            foreach (var borrow in overdueBooks)
            {
                // Send Notification
                // Log with 
            }
        }
    }
}
