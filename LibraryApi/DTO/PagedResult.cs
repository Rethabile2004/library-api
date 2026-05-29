namespace LibraryApi.DTO
{
    public class PagedResult<T>
    {
        // Current page
        public int Page { set; get; }
        // Number of matching items per page
        public int PageSize { set; get; }
        // Number of matching records across all pages
        public int TotalCount { set; get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage { set; get; }
        public bool HasPreviousPage { set; get; }
        public IEnumerable<T> Data { set; get; } = new List<T>();
    }
}
