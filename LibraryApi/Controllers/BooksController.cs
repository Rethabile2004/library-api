using LibraryApi.DTO;
using LibraryApi.Models;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController:ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository=bookRepository;
        }
        [HttpPost]
        public async Task<ActionResult<BookResponseDto>>CreateBook(BookCreateDto createDto)
        {
            var newBook = new Book
            {
                Title=createDto.Title,
                PublishedYear=createDto.PublishedYear,
                Genre=createDto.Genre,
                ISBN=createDto.ISBN,
            };
            await _bookRepository.CreateBookAsync(newBook);
            var success = await _bookRepository.SaveChangesAsync();

            if (!success)
            {
                return StatusCode(500, "Failed to save changes.");
            }
            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, MapToResponse(newBook));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetBookById(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(MapToResponse(book!));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult>DeleteBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return NotFound();
            await _bookRepository.DeleteBookAsync(book);
            var success = await _bookRepository.SaveChangesAsync();

            if (!success)
            {
                return StatusCode(500, "Failed to save changes.");
            }
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(int id, BookCreateDto book)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null) return NotFound();
            existingBook.ISBN = book.ISBN;
            existingBook.Genre = book.Genre;
            existingBook.PublishedYear = book.PublishedYear;
            existingBook.Title = book.Title;

            await _bookRepository.UpdateBookAsync(existingBook);
            var success = await _bookRepository.SaveChangesAsync();

            if (!success)
            {
                return StatusCode(500, "Failed to save changes.");
            }
            return NoContent();
        }
        private static BookResponseDto MapToResponse(Book book)
        {
            return new BookResponseDto
            {
                Genre = book.Genre,
                Id = book.Id,
                PublishedYear = book.PublishedYear,
                Title = book.Title,
                ISBN=book.ISBN
            };
        }
    }
}
