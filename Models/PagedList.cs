using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductAssignment.Models {
    public class PagedList<T> : List<T> {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRecords { get; private set; }
        public int TotalPages { get; private set; }

        public PagedList(List<T> items, int pageNumber, int pageSize, int totalRecords) : base(items) {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
