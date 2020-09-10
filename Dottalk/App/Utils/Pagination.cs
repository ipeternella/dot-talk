using System.Linq;

namespace Dottalk.App.Utils
{
    //
    // Summary:
    //   Parameters used for pagination.
    public class PaginationParams
    {
        private const int maxPageSize = 50;
        public int pageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int pageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public int itemsToSkip
        {
            get { return (pageNumber - 1) * pageSize; }
        }
    }
    //
    // Summary:
    //   Extension used to skip database items from the real database query in order to paginate results.
    public static class PaginationExtension
    {
        public static IQueryable<T> GetPage<T>(this IOrderedQueryable<T> query, PaginationParams paginationParams)
        {
            return query.Skip(paginationParams.itemsToSkip).Take(paginationParams.pageSize);
        }
    }
}