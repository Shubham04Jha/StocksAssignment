using Grpc.Core;
using StocksAssignment.Grpc.Contracts;
using StocksAssignment.GrpcServer.Repositories;

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
            var stocks = await _stockRepository.GetStocksAsync(request);
            var response = new GetStocksResponse();
            response.Stocks.AddRange(stocks);
            return response;
        }
    }
}
