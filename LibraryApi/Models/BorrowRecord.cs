using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BorrowedAt { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime? ReturnedAt { get; set; }
    }
}
