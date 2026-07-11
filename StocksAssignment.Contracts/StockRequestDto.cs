using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.Contracts
{
    public class StockRequestDto
    {
        public string? Fuel { get; set; }

        public string? Car { get; set; }

        public int? City { get; set; }

        public string? Budget { get; set; }
    }
}
