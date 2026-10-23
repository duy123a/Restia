namespace Restia.Common.Shared.Constants
{
    public static class PaginationConstants
    {
        public static readonly int[] PageSizeOptions = [1, 2, 5, 10, 100];
        public const int DefaultPageSize = 10;
        public static int NormalizePageSize(int pageSize) =>
            PageSizeOptions.Contains(pageSize) ? pageSize : DefaultPageSize;
    }
}
