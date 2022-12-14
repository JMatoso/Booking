using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Entities
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = default!;

        [Required, Range(1800, int.MaxValue)]
        public int Year { get; set; }
        public string Genre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Author { get; set; } = default!;

        [Required, StringLength(100)]
        public string Editor { get; set; } = default!;

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        public Book()
        {
            Created = DateTime.Now;
        }
    }
}
