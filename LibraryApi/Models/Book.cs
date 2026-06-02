using System.ComponentModel.DataAnnotations;
namespace LibraryApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string ISBN { get; set; } = string.Empty;
        [Range(1000, 9999)]
        public int PublishedYear { get; set; }
        [Required]
        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}