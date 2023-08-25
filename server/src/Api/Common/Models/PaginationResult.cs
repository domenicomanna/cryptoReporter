namespace Api.Common.Models;

public class PaginationResult<T>
{
    public List<T> Records { get; set; } = new List<T>();
    public int TotalRecordCount { get; set; }
    public int CurrentPageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecordCount / (double)PageSize);
    public bool HasPreviousPage => CurrentPageIndex > 0;
    public bool HasNextPage => CurrentPageIndex + 1 < TotalPages;
}
