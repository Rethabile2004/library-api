using Asp.Versioning;
using LibraryApi.DTO;
using LibraryApi.Models;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace LibraryApi.Controllers
{
    /// <summary>
    /// Manages books for both authenticated and unauthenticated users.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        /// <summary>
        /// Retrieves a paginated list of books based on the specified query parameters.
        /// </summary>
        /// <param name="bookQueryParameters">The parameters used to filter and paginate the list of books.</param>
        /// <returns>A paged result containing the books and pagination information.</returns>
        /// <response code="200">Returns the paginated list of books.</response>
        [HttpGet]
        [EnableRateLimiting("read")]
        [ProducesResponseType(typeof(PagedResult<BookResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<BookResponseDto>>> GetAllBooks([FromQuery] BookQueryParameters bookQueryParameters)
        {
            var (books, totalCount) = await _bookRepository.GetAllAsync(bookQueryParameters);
            var pagedResult = new PagedResult<BookResponseDto>
            {
                Page = bookQueryParameters.Page,
                PageSize = bookQueryParameters.PageSize,
                TotalCount = totalCount,
                Data = books.Select(b => MapToResponse(b))
            };
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves a single book by ID.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book matching the given ID.</returns>
        /// <response code="200">Returns the book.</response>
        /// <response code="404">Book not found.</response>
        [HttpGet("{id}")]
        [EnableRateLimiting("read")]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookResponseDto>> GetBookById(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(MapToResponse(book));
        }

        /// <summary>
        /// Creates a new book. Requires authentication.
        /// </summary>
        /// <param name="createDto">The book details.</param>
        /// <returns>The newly created book.</returns>
        /// <response code="201">Book created successfully.</response>
        /// <response code="401">Authentication required.</response>
        /// <response code="404">Author not found.</response>
        [Authorize]
        [HttpPost]
        [EnableRateLimiting("write")]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookResponseDto>> CreateBook(BookCreateDto createDto)
        {
            var authorExists = await _bookRepository.AuthorExistsAsync(createDto.AuthorId);
            if (!authorExists)
                return NotFound(new { message = $"Author with id {createDto.AuthorId} does not exist." });

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

        /// <summary>
        /// Replaces a book by ID. Requires authentication.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="book">The updated book details.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Book updated successfully.</response>
        /// <response code="401">Authentication required.</response>
        /// <response code="404">Book not found.</response>
        [Authorize]
        [HttpPut("{id}")]
        [EnableRateLimiting("write")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateBook(int id, BookCreateDto book)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null) return NotFound();

            existingBook.ISBN = book.ISBN;
            existingBook.Genre = book.Genre;
            existingBook.PublishedYear = book.PublishedYear;
            existingBook.Title = book.Title;

            await _bookRepository.UpdateBookAsync(existingBook);
            await _bookRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a book by ID. Requires authentication.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Book deleted successfully.</response>
        /// <response code="401">Authentication required.</response>
        /// <response code="404">Book not found.</response>
        [Authorize]
        [HttpDelete("{id}")]
        [EnableRateLimiting("write")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return NotFound();

            await _bookRepository.DeleteBookAsync(book);
            await _bookRepository.SaveChangesAsync();

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
                ISBN = book.ISBN,
                AuthorBio = book.Author?.Bio,
                AuthorName = book.Author?.Name,
                AuthorId = book.AuthorId
            };
        }
    }
}