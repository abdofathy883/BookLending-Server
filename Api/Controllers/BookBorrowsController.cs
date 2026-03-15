using Application.DTOs.Books;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookBorrowsController : ControllerBase
    {
        private readonly IBookBorrowService _bookBorrowService;

        public BookBorrowsController(IBookBorrowService bookBorrowService)
        {
            _bookBorrowService = bookBorrowService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookBorrowDto>>> GetAll()
        {
            var borrows = await _bookBorrowService.GetAll();
            return Ok(borrows);
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<List<BookBorrowDto>>> GetByBookId(int bookId)
        {
            try
            {
                var borrows = await _bookBorrowService.GetByBookId(bookId);
                return Ok(borrows);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("borrow")]
        public async Task<ActionResult<BookBorrowDto>> BorrowBook(BorrowBookDto borrowDto)
        {
            try
            {
                var borrow = await _bookBorrowService.BorrowBook(borrowDto);
                return Ok(borrow);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return")]
        public async Task<ActionResult<BookBorrowDto>> ReturnBook(ReturnBookDto returnDto)
        {
            try
            {
                var borrow = await _bookBorrowService.ReturnBook(returnDto);
                return Ok(borrow);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
