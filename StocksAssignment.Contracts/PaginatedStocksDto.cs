using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.Contracts
{
    public class PaginatedStocksDto
    {
        public List<StockDto> Stocks { get; set; } = [];
        public bool HasNextPage { get; set; }
    }
}
