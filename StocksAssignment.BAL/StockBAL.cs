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

        public async Task<PaginatedStocksDto> GetStocksAsync(Filters filters)
        {
            var originalLimit = filters.Limit;
            if (filters.Limit.HasValue)
            {
                filters.Limit = filters.Limit.Value + 1;
            }

            var stocks = await _stockDAL.GetStocksAsync(filters);

            var hasNextPage = false;
            if (originalLimit.HasValue && stocks.Count > originalLimit.Value)
            {
                hasNextPage = true;
                stocks.RemoveAt(stocks.Count - 1);
            }

            var result = new List<StockDto>();

            foreach (var stock in stocks)
            {
                var dto = _mapper.ToStockDto(stock);

                dto.IsValueForMoney = IsValueForMoney(stock);

                result.Add(dto);
            }

            return new PaginatedStocksDto
            {
                Stocks = result,
                HasNextPage = hasNextPage
            };
        }

        public async Task<List<CityDto>> GetCitiesAsync()
        {
            var cities = await _stockDAL.GetCitiesAsync();
            return cities.Select(c => _mapper.ToCityDto(c)).ToList();
        }

        public async Task<List<MakeDto>> GetMakesAsync()
        {
            var makes = await _stockDAL.GetMakesAsync();
            return makes.Select(m => _mapper.ToMakeDto(m)).ToList();
        }
    }
}
