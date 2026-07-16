using StocksAssignment.Contracts;
using StocksAssignment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.BAL
{
    public interface IStockBAL
    {
        Task<PaginatedStocksDto> GetStocksAsync(Filters filters);
        Task<List<CityDto>> GetCitiesAsync();
        Task<List<MakeDto>> GetMakesAsync();
    }
}
