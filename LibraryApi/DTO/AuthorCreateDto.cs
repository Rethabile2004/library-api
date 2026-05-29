using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTO
{
    public class AuthorCreateDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters.")]
        [Required(ErrorMessage = "Author name is required.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Bio must be less than 500 characters.")]
        public string Bio { get; set; } = string.Empty;
    }
}
