using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTO
{
    public class BookCreateDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Range(1000, 9999, ErrorMessage = "Year must be between 1000 and 9999.")]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Genre must be between 2 and 100 characters.")]
        public string Genre { get; set; } = string.Empty;
        [Required(ErrorMessage = "ISBN is required.")]
        [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "ISBN must be 10 or 13 digits.")]
        public string ISBN { get; set; } = string.Empty;
        public int? AuthorId { get; set; }
    }
}
