using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTO
{
    public class BorrowRecordResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public bool IsReturned => ReturnedAt.HasValue;
    }
}
