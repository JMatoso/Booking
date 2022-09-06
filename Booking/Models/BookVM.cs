using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class BookVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public int Year { get; set; }
        public string Genre { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Editor { get; set; } = default!;
        public DateTime Created { get; set; }
    }
}
