using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Models.Dtos
{
    public class PaginationEntityDto<TEntity>
    {
        public PaginationEntityDto()
        {
        }

        public PaginationEntityDto(int pageIndex, int pageSize) : this()
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
        public List<TEntity> Entities { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public int PageSize { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling(Count / (double)PageSize);
            }
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
