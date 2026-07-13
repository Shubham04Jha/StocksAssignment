using Grpc.Core;
using StocksAssignment.Grpc.Contracts;
using StocksAssignment.GrpcServer.Repositories;
using System;
using StocksAssignment.Domain.Exceptions;

namespace StocksAssignment.Grpc.Services
{
    public class StocksService: Stocks.StocksBase
    {
        private readonly ILogger<StocksService> _logger;
        private readonly IStockRepository _stockRepository;

        public StocksService(ILogger<StocksService> logger, IStockRepository stockRepository)
        {
            _logger = logger;
            _stockRepository = stockRepository;
        }

        public override async Task<GetStocksResponse> GetStocks(GetStocksRequest request, ServerCallContext context)
        {
            try
            {
                var stocks = await _stockRepository.GetStocksAsync(request);
                var response = new GetStocksResponse();
                response.Stocks.AddRange(stocks);
                return response;
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database failure occurred during gRPC service call.");
                throw new RpcException(new Status(StatusCode.Unavailable, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unhandled exception occurred in the gRPC service.");
                throw new RpcException(new Status(StatusCode.Internal, "Internal gRPC server error."));
            }
        }

        public override async Task<GetCitiesResponse> GetCities(GetCitiesRequest request, ServerCallContext context)
        {
            try
            {
                var cities = await _stockRepository.GetCitiesAsync(request);
                var response = new GetCitiesResponse();
                response.Cities.AddRange(cities);
                return response;
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database failure occurred during gRPC service call.");
                throw new RpcException(new Status(StatusCode.Unavailable, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unhandled exception occurred in the gRPC service.");
                throw new RpcException(new Status(StatusCode.Internal, "Internal gRPC server error."));
            }
        }

        public override async Task<GetMakesResponse> GetMakes(GetMakesRequest request, ServerCallContext context)
        {
            try
            {
                var makes = await _stockRepository.GetMakesAsync(request);
                var response = new GetMakesResponse();
                response.Makes.AddRange(makes);
                return response;
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "Database failure occurred during gRPC service call.");
                throw new RpcException(new Status(StatusCode.Unavailable, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unhandled exception occurred in the gRPC service.");
                throw new RpcException(new Status(StatusCode.Internal, "Internal gRPC server error."));
            }
        }
    }
}
