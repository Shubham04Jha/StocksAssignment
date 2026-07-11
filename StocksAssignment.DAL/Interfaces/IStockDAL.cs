using StocksAssignment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.DAL
{
    public interface IStockDAL
    {
        Task<List<Stock>> GetStocksAsync(Filters filters);
    }
}
