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
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowBookRepository _borrowBook;
        private readonly ILogger<BorrowController> _logger;

        public BorrowController(IBorrowBookRepository borrowBook, ILogger<BorrowController> logger)
        {
            _borrowBook = borrowBook;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all borrow records for the authenticated user.
        /// </summary>
        /// <returns>A list of borrow records belonging to the current user.</returns>
        /// <response code="200">Returns the list of borrow records.</response>
        /// <response code="401">Authentication required.</response>
        [HttpGet("my-books")]
        [EnableRateLimiting("read")]
        [ProducesResponseType(typeof(IEnumerable<BorrowRecordResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllRecords()
        {
            var records = await _borrowBook.GetAllAsync(GetUserId());
            return Ok(records.Select(r => MapToResponse(r)));
        }

        /// <summary>
        /// Retrieves a single borrow record by ID.
        /// </summary>
        /// <param name="id">The ID of the borrow record.</param>
        /// <returns>The borrow record matching the given ID.</returns>
        /// <response code="200">Returns the borrow record.</response>
        /// <response code="401">Authentication required.</response>
        /// <response code="403">Record belongs to a different user.</response>
        /// <response code="404">Borrow record not found.</response>
        [HttpGet("{id}")]
        [EnableRateLimiting("read")]
        [ProducesResponseType(typeof(BorrowRecordResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BorrowRecordResponseDto>> GetRecordById(int id)
        {
            var existing = await _borrowBook.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (existing.UserId != GetUserId()) return Forbid();
            return Ok(MapToResponse(existing));
        }

        /// <summary>
        /// Borrows a book for the authenticated user.
        /// </summary>
        /// <param name="bookId">The ID of the book to borrow.</param>
        /// <returns>The newly created borrow record.</returns>
        /// <response code="201">Book borrowed successfully.</response>
        /// <response code="401">Authentication required.</response>
        /// <response code="404">Book not found.</response>
        /// <response code="409">Book is currently borrowed by another user.</response>
        [HttpPost("{bookId}")]
        [EnableRateLimiting("write")]
        [ProducesResponseType(typeof(BorrowRecordResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BorrowRecordResponseDto>> BorrowBook(int bookId)
        {
            var book = await _borrowBook.GetBookByIdAsync(bookId);
            if (book == null)
            {
                _logger.LogWarning("User {UserId} attempted to borrow unavailable book {BookId}", GetUserId(), bookId);
                return NotFound(new { message = "Book not found." });
            }

            var alreadyBorrowed = await _borrowBook.IsBookCurrentlyBorrowedAsync(bookId);
            if (alreadyBorrowed)
            {
                _logger.LogWarning("User {UserId} attempted to borrow a currently borrowed book {BookId}", GetUserId(), bookId);
                return Conflict(new { message = "This book is currently borrowed." });
            }

            var newRecord = new BorrowRecord
            {
                BookId = bookId,
                UserId = GetUserId(),
                BorrowedAt = DateTime.UtcNow,
                ReturnedAt = null
            };

            await _borrowBook.BorrowAsync(newRecord);
            await _borrowBook.SaveChangesAsync();
            _logger.LogInformation("Book borrowed. UserId={UserId}, BookId={BookId}", GetUserId(), bookId);

            return CreatedAtAction(nameof(GetRecordById), new { id = newRecord.Id }, MapToResponse(newRecord));
        }

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        /// <param name="bookId">The ID of the book to return.</param>
        /// <returns>The updated borrow record with the return timestamp.</returns>
        /// <response code="200">Book returned successfully.</response>
        /// <response code="401">Authentication required.</response>
        /// <response code="404">No active borrow record found for this book.</response>
        [HttpPatch("{bookId}/return")]
        [EnableRateLimiting("write")]
        [ProducesResponseType(typeof(BorrowRecordResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BorrowRecordResponseDto>> ReturnBook(int bookId)
        {
            var record = await _borrowBook.GetActiveBorrowAsync(bookId, GetUserId());
            if (record == null)
            {
                _logger.LogWarning("User {UserId} attempted to return non-existing book {BookId}", GetUserId(), bookId);
                return NotFound(new { message = "Active borrow record not found." });
            }

            record.ReturnedAt = DateTime.UtcNow;
            await _borrowBook.UpdateAsync(record);
            await _borrowBook.SaveChangesAsync();
            _logger.LogInformation("User {UserId} returned a book {BookId}", GetUserId(), bookId);

            return Ok(MapToResponse(record));
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private static BorrowRecordResponseDto MapToResponse(BorrowRecord record)
        {
            return new BorrowRecordResponseDto
            {
                BookId = record.BookId,
                BorrowedAt = record.BorrowedAt,
                Id = record.Id,
                ReturnedAt = record.ReturnedAt,
                BookISBN = record.Book?.ISBN,
                BookTitle = record.Book?.Title
            };
        }
    }
}