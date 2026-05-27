using LibraryApi.Data;
using LibraryApi.Models;

namespace LibraryApi.Repositories
{
    public class BookRepository:IBookRepository
    {
        private readonly AppDbContext _context;
        public  BookRepository(AppDbContext context)
        {
            _context = context;
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

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
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
