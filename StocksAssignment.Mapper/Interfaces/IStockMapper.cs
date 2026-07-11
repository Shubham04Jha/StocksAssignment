using StocksAssignment.Contracts;
using StocksAssignment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.Mapper
{
    public interface IStockMapper
    {
        Filters ToFilters(StockRequestDto dto);
        StockDto ToStockDto(Stock stock);
    }
}
