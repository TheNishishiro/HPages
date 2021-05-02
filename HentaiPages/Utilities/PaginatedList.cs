using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HentaiPages.Utilities
{
    /// <summary>
    /// Create a paged result from database query 
    /// </summary>
    /// <typeparam name="T">Type of table to create page for</typeparam>
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static int CalculateIndex(int id, int page, int pageSize)
        {
            return ((page - 1) * pageSize) + (id - 1);
        }

        /// <summary>
        /// Create page for paginated view
        /// </summary>
        /// <param name="source">Query to base page off of</param>
        /// <param name="pageIndex">Page to show off</param>
        /// <param name="pageSize">Amount of records per page</param>
        /// <returns></returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
