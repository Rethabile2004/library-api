using Asp.Versioning;
using LibraryApi.DTO;
using LibraryApi.Models;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BooksController:ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<BookResponseDto>>> GetAllBooks([FromQuery]BookQueryParameters bookQueryParameters)
        {
            var userId = GetCurrentUserId();
            var (books,totalCount) = await _bookRepository.GetAllAsync(bookQueryParameters, userId);
            var pagedResult = new PagedResult<BookResponseDto>
            {
                Page = bookQueryParameters.Page,
                PageSize = bookQueryParameters.PageSize,
                TotalCount = totalCount,
                Data = books.Select(b => MapToResponse(b))
            };
            return Ok(pagedResult);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BookResponseDto>> CreateBook(BookCreateDto createDto)
        {
            var authorExists = await _bookRepository.AuthorExistsAsync(createDto.AuthorId);
            if (!authorExists)
            {
                return NotFound(new { message = $"Author with id {createDto.AuthorId} does not exist." });
            }

            var newBook = new Book
            {
                Title = createDto.Title,
                PublishedYear = createDto.PublishedYear,
                Genre = createDto.Genre,
                ISBN = createDto.ISBN,
                AuthorId = createDto.AuthorId,
                
            };

            await _bookRepository.CreateBookAsync(newBook);
            await _bookRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, MapToResponse(newBook));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetBookById(int id)
        {
            var userId = GetCurrentUserId();
            var book = await _bookRepository.GetByIdAsync(id, userId);
            if (book == null) return NotFound();
            return Ok(MapToResponse(book!));
        }
        [Authorize]
        [HttpDelete("{id}")] 
        public async Task<ActionResult>DeleteBook(int id)
        {
            var userId = GetCurrentUserId();
            var book = await _bookRepository.GetByIdAsync(id, userId);
            if (book == null) return NotFound();
            await _bookRepository.DeleteBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            return NoContent();
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(int id, BookCreateDto book)
        {
            var userId = GetCurrentUserId();
            var existingBook = await _bookRepository.GetByIdAsync(id, userId);
            if (existingBook == null) return NotFound();
            existingBook.ISBN = book.ISBN;
            existingBook.Genre = book.Genre;
            existingBook.PublishedYear = book.PublishedYear;
            existingBook.Title = book.Title;

            await _bookRepository.UpdateBookAsync(existingBook);
            await _bookRepository.SaveChangesAsync();

            return NoContent();
        }
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        private static BookResponseDto MapToResponse(Book book)
        {
            return new BookResponseDto
            {
                Genre = book.Genre,
                Id = book.Id,
                PublishedYear = book.PublishedYear,
                Title = book.Title,
                ISBN=book.ISBN,
                AuthorBio=book.Author?.Bio,
                AuthorName=book.Author?.Name,
                AuthorId=book.AuthorId
            };
        }
    }
}
