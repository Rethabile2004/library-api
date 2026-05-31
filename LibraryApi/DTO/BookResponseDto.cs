using LibraryApi.Models;

namespace LibraryApi.DTO
{
    public class BookResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int PublishedYear { get; set; }

        public string Genre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string? AuthorName { get; set; } = string.Empty;
        public string? AuthorBio { get; set; } = string.Empty;
        public int? AuthorId { get; set; }
    }
}
