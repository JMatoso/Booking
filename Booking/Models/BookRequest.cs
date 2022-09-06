using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class BookRequest
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = default!;

        [Required, Range(1800, int.MaxValue)]
        public int Year { get; set; }
        public string Genre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Author { get; set; } = default!;

        [Required, StringLength(100)]
        public string Editor { get; set; } = default!;
    }
}
