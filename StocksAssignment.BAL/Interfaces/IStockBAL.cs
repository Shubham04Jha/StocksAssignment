using StocksAssignment.Contracts;
using StocksAssignment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.BAL
{
    public interface IStockBAL
    {
        Task<List<StockDto>> GetStocksAsync(Filters filters);
    }
}
