using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.Contracts
{
    public class PaginatedStocksResponse
    {
        public string? NextPageUrl { get; set; }
        public List<StockDto> Stocks { get; set; } = [];
    }
}
