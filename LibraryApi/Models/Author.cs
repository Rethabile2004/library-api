using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }
}