using LibraryApi.DTO;
using LibraryApi.Models;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;
using System.Security.Claims;

namespace LibraryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowController:ControllerBase
    {
        public readonly IBorrowBookRepository _borrowBook;
        private readonly ILogger<BorrowController> _logger;
        public BorrowController(IBorrowBookRepository borrowBook, ILogger<BorrowController> logger)
        {
            _borrowBook = borrowBook;
            _logger = logger;
        }
        [HttpGet("my-books")]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllRecords()
        {
            var records = await _borrowBook.GetAllAsync(GetUserId());
            return Ok(records.Select(r => MapToResponse(r)));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>>GetRecordById(int id)
        {
            var existing = await _borrowBook.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }
            if (existing.UserId != GetUserId())
            {
                return Forbid();
            }
            return Ok(MapToResponse(existing));
        }
        [HttpPost("{bookId}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> BorrowBook(int bookId)
        {
            var book = await _borrowBook.GetBookByIdAsync(bookId);
            if (book == null)
            {
                _logger.LogWarning("User {UserId} attempted to borrow unavailable book {BookId}",GetUserId(),bookId);
                return NotFound(new
                {
                    message = "Book not found."
                });
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
        [HttpPatch("{bookId}/return")]
        public async Task<ActionResult<BorrowRecordResponseDto>> ReturnBook(int bookId)
        {
            var record = await _borrowBook.GetActiveBorrowAsync(bookId, GetUserId());
            if (record == null)
            {
                _logger.LogWarning("User {UserId} attempted to return non-existing book {BookId}", GetUserId(), bookId);
                return NotFound(new
                {
                    message = "Active borrow record not found."
                });
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
                UserId = record.UserId
            };
        }
    }
}
