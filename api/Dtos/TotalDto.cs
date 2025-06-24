using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class TotalDto
    {
        public int totalAmount { get; set; }
        public int totalOrder { get; set; }
        public int totalCustomer { get; set; }
        public int totalPendingOrder { get; set; }
    }
}