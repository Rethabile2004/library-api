using LibraryApi.Models;

namespace LibraryApi.Repositories
{
    public interface IBorrowBookRepository
    {
        Task<IEnumerable<BorrowRecord>> GetAllAsync(int userId);
        Task<BorrowRecord> BorrowAsync(BorrowRecord borrowRecord);
        Task<BorrowRecord?> GetByIdAsync(int id);
        Task UpdateAsync(BorrowRecord borrowRecord);
        Task DeleteAsync(BorrowRecord borrowRecord);
        Task<bool> IsBookCurrentlyBorrowedAsync(int bookId);
        Task<BorrowRecord?> GetActiveBorrowAsync(int id, int userId);
        Task<BorrowRecord?> GetBookByIdAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
