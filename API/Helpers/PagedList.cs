using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    //pagelist to work with any type of object or class - <T>
    public class PagedList<T>: List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize) 
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items); // access to items from the page list from the list itself.
        }

        public int CurrentPage { get; }
        public int TotalPages { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        // Create a page list of any type of object
        // IQuery - passing query built up in memory by entity framework but yet to execute against database.
        //Async() method executes query on database.
        // It counts the number of items from this "source" query , get item from database using skip and take operations and execute into a List.
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}