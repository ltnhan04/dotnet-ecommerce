using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class RevenueDto
    {
        public string label { get; set; } = string.Empty;
        public int totalRevenue { get; set; }
    }
}