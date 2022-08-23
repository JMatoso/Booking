using Booking.Entities;

namespace Booking.Repository
{
    public interface IBookRepository
    {
        Task Add(Book book);
        Task Delete(int bookId);
        Task<IEnumerable<Book>> Get();
        Task<Book?> Get(int bookId);
        Task<IEnumerable<Book>> Get(string keyword);
        Task Update(Book book);
    }
}