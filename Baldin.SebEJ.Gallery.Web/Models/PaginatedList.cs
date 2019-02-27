using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Web.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
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

        public IEnumerable<int> GetRoutePages(int span = 2)
        {
            var pages = new int[span * 2 + 1];
            var cnt = PageIndex - span;
            var counter = 0;
            while(cnt <= TotalPages && counter < pages.Length)
            {
                if (cnt <= 0)
                {
                    cnt++;
                    continue;
                }
                pages[counter] = cnt;
                cnt++;
                counter++;
            }
            return pages.TakeWhile(x => x != 0);
        }

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
            {
                return new PaginatedList<T>(new T[1], 1, 1, 0);
            }
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
