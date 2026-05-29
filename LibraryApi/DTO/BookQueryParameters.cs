using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTO
{
    public class BookQueryParameters
    {
        public string? SearchTitle { get; set; }
        //public string? ISBN { get; set; }
        public int? PublishedYear { get; set; }
        public string? Genre { get; set; }
        public string SortBy { get; set; } = "id";
        private int _page = 1;
        public int Page
        {
            get => _page;
            // If client sends page=0 or negative, default to 1
            set => _page = value < 1 ? 1 : value;
        }

        // Results per page — capped at 50 to prevent abuse
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            // If client sends pageSize=0 or over 50, clamp it
            set => _pageSize = value < 1 ? 10 : value > 50 ? 50 : value;
        }
    }
}