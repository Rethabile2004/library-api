using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories
{
    public class BorrowBookRepository : IBorrowBookRepository
    {
        private readonly AppDbContext _context;
        public BorrowBookRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<BorrowRecord> BorrowAsync(BorrowRecord borrowRecord)
        {
            await _context.BorrowRecords.AddAsync(borrowRecord);
            return borrowRecord;
        }

        public Task DeleteAsync(BorrowRecord borrowRecord)
        {
            _context.BorrowRecords.Remove(borrowRecord);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<BorrowRecord>> GetAllAsync(int userId)
        {
            return await _context.BorrowRecords.Include(b=>b.Book).Where(r => r.UserId == userId).ToListAsync();
        }

        public Task<BorrowRecord?> GetByIdAsync(int id)
        {
            return _context.BorrowRecords.Include(b=>b.Book).FirstOrDefaultAsync(br=>br.Id==id);
        }

        public async Task<BorrowRecord?> GetActiveBorrowAsync(int id, int userId)
        {
            return await _context.BorrowRecords.Include(b => b.Book).
                FirstOrDefaultAsync(r => r.BookId == id && r.UserId == userId && r.ReturnedAt == null);
        }
        public Task UpdateAsync(BorrowRecord borrowRecord)
        {
            _context.BorrowRecords.Update(borrowRecord);
            return Task.CompletedTask;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> IsBookCurrentlyBorrowedAsync(int bookId)
        {
            return await _context.BorrowRecords
                .AnyAsync(r => r.BookId == bookId && r.ReturnedAt == null);
        }
        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
