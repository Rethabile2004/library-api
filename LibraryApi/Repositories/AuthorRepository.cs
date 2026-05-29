using LibraryApi.Data;
using LibraryApi.DTO;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;
        public AuthorRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<Author> Authors, int TotalCount)> GetAllAsync(AuthorQueryParameters queryParameters)
        {
            var query = _context.Authors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchName))
            {
                query = query.Where(a => a.Name.Contains(queryParameters.SearchName.ToLower()));
            }
            int totalCount = await query.CountAsync();
            query = queryParameters.SortBy switch
            {
                "name" => query.OrderBy(a => a.Name),
                _=> query.OrderBy(a => a.Id)
            };
            var authors = await query.Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize).ToListAsync();
            return (authors,totalCount);
        }
        public async Task<Author> CreateAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
            return author;
        }
        public async Task<Author?> GetByIdAsync(int id)
        {
            Author? author = await _context.Authors.FindAsync(id);
            return author==null?null:author;
        }
        public Task DeleteAsync(Author author)
        {
             _context.Authors.Remove(author);
            return Task.CompletedTask;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}