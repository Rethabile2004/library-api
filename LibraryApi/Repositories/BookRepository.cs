using LibraryApi.Data;
using LibraryApi.DTO;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories
{
    public class BookRepository:IBookRepository
    {
        private readonly AppDbContext _context;
        public  BookRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<Book> Books, int TotalCount)>GetAllAsync(BookQueryParameters queryParameters,int userId)
        {
            // base query
            var query = _context.Books.Include(a=>a.Author).AsQueryable();
            query = query.Where(b => b.Id == userId);
            // apply filters
            if(!string.IsNullOrWhiteSpace(queryParameters.SearchTitle))
            {
                query = query.Where(b => b.Title.Contains(queryParameters.SearchTitle));
            }
            if (queryParameters.PublishedYear.HasValue)
            {
                query=query.Where(b=>b.PublishedYear==queryParameters.PublishedYear);
            }
            if (!string.IsNullOrWhiteSpace(queryParameters.Genre))
            {
                query=query.Where(b=> b.Genre.ToLower() == queryParameters.Genre.ToLower());
            }
            var totalCount = await query.CountAsync();
            // apply sorting
            query=queryParameters.SortBy.ToLower() switch
            {
                "title"=>query.OrderBy(b=>b.Title),
                "publishedyear" => query.OrderBy(b=>b.PublishedYear),
                "genre"=> query.OrderBy(b => b.Genre),

                //"createdat"=>query.OrderByDescending(b=>b.CreatedAt),
                _ => query.OrderBy(b => b.Id)
            };
            var books = await query.Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize).ToListAsync();
            return (books, totalCount);
        }
        public async Task<Book> CreateBookAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            return book;
        }

        public Task DeleteBookAsync(Book book)
        {
            _context.Books.Remove(book);
            return Task.CompletedTask;
        }

        public async Task<Book?> GetByIdAsync(int id, int userId)
        {
            return await _context.Books.Include(a => a.Author).
                FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            return Task.CompletedTask;
        }
    }
}
