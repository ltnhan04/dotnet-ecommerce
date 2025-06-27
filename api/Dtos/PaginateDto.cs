using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class PaginateDto<Data>
    {
        public int total { get; set; }
        public int page { get; set; }
        public int currentPage { get; set; }
        public List<Data> items { get; set; } = new();
    }
}