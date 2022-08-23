using Booking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
