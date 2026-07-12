using StocksAssignment.Domain.Entities;
using StocksAssignment.Domain.Enums;
using GrpcStock = StocksAssignment.Grpc.Contracts.Stock;

namespace StocksAssignment.DAL.Mappings
{
    public static class StockMappingExtensions
    {
        public static Stock ToDomain(this GrpcStock stock)
        {
            return new Stock
            {
                Id = stock.StockId,
                MakeId = stock.MakeId,
                MakeName = stock.MakeName,
                ModelName = stock.ModelName,
                RegistrationYear = stock.RegistrationYear,
                FuelType = (FuelType)stock.FuelType,
                Price = stock.Price,
                KilometersDriven = stock.KilometersDriven,
                CityId = stock.CityId
            };
        }
    }
}
