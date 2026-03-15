using Application.DTOs.Books;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAll();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            try
            {
                var book = await _bookService.GetById(id);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create(CreateBookDto newBook)
        {
            var book = await _bookService.Create(newBook);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
    }
}
