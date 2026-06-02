using LibraryApi.DTO;
using LibraryApi.Models;

namespace LibraryApi.Repositories
{
    public interface IBookRepository
    {
        Task<(IEnumerable<Book> Books,int TotalCount)>GetAllAsync(BookQueryParameters queryParameters, int userId);
        Task<Book?> GetByIdAsync(int id, int userId);
        Task<Book> CreateBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Book book);
        Task<bool> SaveChangesAsync();
        Task<bool> AuthorExistsAsync(int authorId);
    }
}
