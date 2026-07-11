using StocksAssignment.Grpc.Contracts;

namespace StocksAssignment.GrpcServer.Repositories
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetStocksAsync(GetStocksRequest request);
    }
}
