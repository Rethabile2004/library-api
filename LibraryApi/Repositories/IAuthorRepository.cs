using LibraryApi.DTO;
using LibraryApi.Models;

namespace LibraryApi.Repositories
{
    public interface IAuthorRepository
    {
        Task<(IEnumerable<Author>Authors,int TotalCount)> GetAllAsync(AuthorQueryParameters queryParameters);
        Task<Author?> GetByIdAsync(int id);
        Task<Author> CreateAsync(Author author);
        Task DeleteAsync(Author author);
        Task<bool> SaveChangesAsync();
    }
}
