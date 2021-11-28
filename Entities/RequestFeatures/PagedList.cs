using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public PaginationMetaData PaginationMetaData { get; private set; }

        public PagedList(IEnumerable<T> pageItems, int totalCount, int pageSize, int pageNumber)
        {
            PaginationMetaData = new PaginationMetaData()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int) Math.Ceiling(totalCount / (double) pageSize)
            };

            this.AddRange(pageItems);
        }


        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageSize, int pageNumber)
        {
            var totalCount = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return new PagedList<T>(items, totalCount, pageSize, pageNumber);
        }
    }
}
