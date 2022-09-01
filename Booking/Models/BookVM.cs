using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Booking.Models
{
    public class BookVM
    {
        [Required, StringLength(100)]
        public string Title { get; set; }

        [Required, Range(1800, int.MaxValue)]
        public int Year { get; set; }
        public string Genre { get; set; }

        [Required, StringLength(100)]
        public string Author { get; set; }

        [Required, StringLength(100)]
        public string Editor { get; set; }
    }
}
