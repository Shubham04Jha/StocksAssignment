using StocksAssignment.Contracts;
using StocksAssignment.DAL;
using StocksAssignment.Domain.Entities;
using StocksAssignment.Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.BAL
{
    public class StockBAL : IStockBAL
    {
        private readonly IStockDAL _stockDAL;
        private readonly IStockMapper _mapper;

        public StockBAL(IStockDAL stockDAL, IStockMapper mapper)
        {
            _stockDAL = stockDAL;
            _mapper = mapper;
        }

        private bool IsValueForMoney(Stock stock)
        {
            return stock.KilometersDriven < 10000 && stock.Price < 200000;
        }

        public async Task<List<StockDto>> GetStocksAsync(Filters filters)
        {
            var stocks = await _stockDAL.GetStocksAsync(filters);

            var result = new List<StockDto>();

            foreach (var stock in stocks)
            {
                var dto = _mapper.ToStockDto(stock);

                dto.IsValueForMoney = IsValueForMoney(stock);

                result.Add(dto);
            }

            return result;
        }
    }
}
