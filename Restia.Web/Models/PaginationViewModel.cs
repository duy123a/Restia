using X.PagedList;

namespace Restia.Web.Models
{
    public class PaginationViewModel<T>
    {
        public IPagedList<T> Items { get; set; } = null!;
        public string? Search { get; set; }
        public int PageSize { get; set; }
        public string SearchPlaceholderKey { get; set; } = string.Empty;
    }
}
