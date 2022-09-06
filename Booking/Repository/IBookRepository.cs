using Booking.Entities;

namespace Booking.Repository
{
    public interface IBookRepository
    {
        Task Add(Book book);
        Task Delete(Guid bookId);
        Task<IEnumerable<Book>> Get();
        Task<Book?> Get(Guid bookId);
        Task<IEnumerable<Book>> Get(string keyword);
        Task Update(Book book);
    }
}