using App.Metrics;
using Booking.Data;
using Booking.Entities;
using Booking.Metrics;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly IMetrics _metrics;
        private readonly ApplicationDbContext _dbContext;
        public BookRepository(ApplicationDbContext dbContext, IMetrics metrics)
        {
            _dbContext = dbContext;
            _metrics = metrics;

            _metrics.Measure.Counter.Increment(MetricRegistry.CreatedDbContextCounter);
        }

        public async Task<IEnumerable<Book>> Get()
        {
            return await _dbContext.Books.ToListAsync();
        }

        public async Task<IEnumerable<Book>> Get(string keyword)
        {
            return await _dbContext.Books
                .Where(x =>
                    x.Title.Contains(keyword) ||
                    x.Author.Contains(keyword) ||
                    x.Editor.Contains(keyword) ||
                    x.Genre.Contains(keyword))
                .ToListAsync();
        }

        public async Task<Book?> Get(Guid bookId)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == bookId);
        }

        public async Task Add(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();

            _metrics.Measure.Counter.Increment(MetricRegistry.CreatedBooksCounter);
        }

        public async Task Update(Book book)
        {
            _dbContext.Update(book);
            await _dbContext.SaveChangesAsync();

            _metrics.Measure.Counter.Increment(MetricRegistry.UpdatedBooksCounter);
        }

        public async Task Delete(Guid bookId)
        {
            var book = await Get(bookId);

            if (book is not null)
            {
                _dbContext.Remove(book);
                await _dbContext.SaveChangesAsync();

                _metrics.Measure.Counter.Increment(MetricRegistry.DeletedBooksCounter);
            }
        }
    }
}
