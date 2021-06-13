using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plan_io_T.Models {
    public class DataPageViewModel {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public DataPageViewModel(int count, int pageNumber, int pageSize) {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage {
            get {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage {
            get {
                return (PageNumber < TotalPages);
            }
        }
    }
}
