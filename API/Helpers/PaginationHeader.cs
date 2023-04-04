using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{   // This will be an object that returns inside the HTTP response header in client-side.
    //When response goes back to client, it will be able to obtain pagination details from pagination header from HTTP response.
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}