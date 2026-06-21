using LibraryApi.DTO;
using LibraryApi.Models;

namespace LibraryApi.Repositories
{
    public interface IBookRepository
    {
        Task<(IEnumerable<Book> Books,int TotalCount)>GetAllAsync(BookQueryParameters queryParameters);
        Task<Book?> GetByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Book book);
        Task<bool> SaveChangesAsync();
        Task<bool> AuthorExistsAsync(int authorId);
    }
}
