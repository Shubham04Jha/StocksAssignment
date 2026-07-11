using StocksAssignment.Domain.Entities;
using StocksAssignment.Domain.Enums;
using StocksAssignment.Domain.Exceptions;
using StocksAssignment.Grpc.Contracts;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

using DomainStock = StocksAssignment.Domain.Entities.Stock;

namespace StocksAssignment.DAL
{
    public class StockDAL : IStockDAL
    {
        private readonly Stocks.StocksClient _client;

        public StockDAL(Stocks.StocksClient client)
        {
            _client = client;
        }
        public async Task<List<DomainStock>> GetStocksAsync(Filters filters)
        {
            var request = new GetStocksRequest();

            request.FuelTypes.AddRange(
                filters.FuelTypes.Select(f => (int)f));

            request.MakeIds.AddRange(filters.MakeIds);

            if (filters.MinBudgetLakhs.HasValue)
                request.MinBudgetLakhs = filters.MinBudgetLakhs.Value;

            if (filters.MaxBudgetLakhs.HasValue)
                request.MaxBudgetLakhs = filters.MaxBudgetLakhs.Value;

            if (filters.CityId.HasValue)
                request.CityId = filters.CityId.Value;

            GetStocksResponse response;
            try
            {
                response = await _client.GetStocksAsync(request);
            }
            catch (RpcException ex)
            {
                throw new ServiceUnavailableException("The remote stock retrieval service is currently unavailable.", ex);
            }

            var stocks = response.Stocks
                .Select(s => new DomainStock
                {
                    Id = s.StockId,
                    MakeId = s.MakeId,
                    MakeName = s.MakeName,
                    ModelName = s.ModelName,
                    RegistrationYear = s.RegistrationYear,
                    FuelType = (FuelType)s.FuelType,
                    Price = (decimal)s.Price,
                    KilometersDriven = s.KilometersDriven,
                    CityId = s.CityId
                })
                .ToList();

            return stocks;
        }
    }
}
