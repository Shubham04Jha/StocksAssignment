using StocksAssignment.Domain.Entities;
using StocksAssignment.Domain.Enums;
using StocksAssignment.Domain.Exceptions;
using StocksAssignment.Grpc.Contracts;
using StocksAssignment.DAL.Mappings;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

using DomainStock = StocksAssignment.Domain.Entities.Stock;
using DomainCity = StocksAssignment.Domain.Entities.City;
using DomainMake = StocksAssignment.Domain.Entities.Make;

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

            request.Sc = (int)filters.SortColumn;
            request.So = (int)filters.SortOrder;

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
                .Select(s => s.ToDomain())
                .ToList();

            return stocks;
        }

        public async Task<List<DomainCity>> GetCitiesAsync()
        {
            var request = new GetCitiesRequest();
            GetCitiesResponse response;
            try
            {
                response = await _client.GetCitiesAsync(request);
            }
            catch (RpcException ex)
            {
                throw new ServiceUnavailableException("The remote cities retrieval service is currently unavailable.", ex);
            }

            return response.Cities.Select(c => c.ToDomain()).ToList();
        }

        public async Task<List<DomainMake>> GetMakesAsync()
        {
            var request = new GetMakesRequest();
            GetMakesResponse response;
            try
            {
                response = await _client.GetMakesAsync(request);
            }
            catch (RpcException ex)
            {
                throw new ServiceUnavailableException("The remote makes retrieval service is currently unavailable.", ex);
            }

            return response.Makes.Select(m => m.ToDomain()).ToList();
        }
    }
}
