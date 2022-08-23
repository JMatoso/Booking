#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Entities
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [Required, Range(1800, int.MaxValue)]
        public string Year { get; set; }
        public string Genre { get; set; }

        [Required, StringLength(100)]
        public string Author { get; set; }

        [Required, StringLength(100)]
        public string Editor { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        public Book()
        {
            Created = DateTime.Now;
        }
    }
}
