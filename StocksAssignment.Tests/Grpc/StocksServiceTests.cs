using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using StocksAssignment.Domain.Exceptions;
using StocksAssignment.Grpc.Contracts;
using StocksAssignment.Grpc.Services;
using StocksAssignment.GrpcServer.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StocksAssignment.Tests.Grpc
{
    public class StocksServiceTests
    {
        private readonly Mock<IStockRepository> _mockRepo;
        private readonly Mock<ILogger<StocksService>> _mockLogger;
        private readonly StocksService _service;

        public StocksServiceTests()
        {
            _mockRepo = new Mock<IStockRepository>();
            _mockLogger = new Mock<ILogger<StocksService>>();
            _service = new StocksService(_mockLogger.Object, _mockRepo.Object);
        }

        #region GetStocks Tests

        [Fact]
        public async Task GetStocks_Success_ReturnsGetStocksResponse()
        {
            
            var request = new GetStocksRequest();
            var stocksList = new List<StocksAssignment.Grpc.Contracts.Stock>
            {
                new StocksAssignment.Grpc.Contracts.Stock { StockId = 1, MakeName = "Honda", ModelName = "Civic", Price = 800000 },
                new StocksAssignment.Grpc.Contracts.Stock { StockId = 2, MakeName = "Toyota", ModelName = "Corolla", Price = 900000 }
            };

            _mockRepo.Setup(r => r.GetStocksAsync(request))
                .ReturnsAsync(stocksList);

            var response = await _service.GetStocks(request, null!);

            
            Assert.NotNull(response);
            Assert.Equal(2, response.Stocks.Count);
            Assert.Equal("Honda", response.Stocks[0].MakeName);
            Assert.Equal("Toyota", response.Stocks[1].MakeName);
        }

        [Fact]
        public async Task GetStocks_DatabaseException_ThrowsUnavailableRpcException()
        {
            
            var request = new GetStocksRequest();
            _mockRepo.Setup(r => r.GetStocksAsync(request))
                .ThrowsAsync(new DatabaseException("Database connection failure."));

            var rpcException = await Assert.ThrowsAsync<RpcException>(() => _service.GetStocks(request, null!));
            Assert.Equal(StatusCode.Unavailable, rpcException.StatusCode);
            Assert.Contains("Database connection failure.", rpcException.Status.Detail);
        }

        [Fact]
        public async Task GetStocks_GenericException_ThrowsInternalRpcException()
        {
            
            var request = new GetStocksRequest();
            _mockRepo.Setup(r => r.GetStocksAsync(request))
                .ThrowsAsync(new Exception("Some unexpected error."));

            var rpcException = await Assert.ThrowsAsync<RpcException>(() => _service.GetStocks(request, null!));
            Assert.Equal(StatusCode.Internal, rpcException.StatusCode);
            Assert.Contains("Internal gRPC server error.", rpcException.Status.Detail);
        }

        #endregion

        #region GetCities Tests

        [Fact]
        public async Task GetCities_Success_ReturnsGetCitiesResponse()
        {
            var request = new GetCitiesRequest();
            var citiesList = new List<StocksAssignment.Grpc.Contracts.City>
            {
                new StocksAssignment.Grpc.Contracts.City { CityId = 1, CityName = "Delhi" },
                new StocksAssignment.Grpc.Contracts.City { CityId = 2, CityName = "Mumbai" }
            };

            _mockRepo.Setup(r => r.GetCitiesAsync(request))
                .ReturnsAsync(citiesList);

            var response = await _service.GetCities(request, null!);

            Assert.NotNull(response);
            Assert.Equal(2, response.Cities.Count);
            Assert.Equal("Delhi", response.Cities[0].CityName);
            Assert.Equal("Mumbai", response.Cities[1].CityName);
        }

        [Fact]
        public async Task GetCities_DatabaseException_ThrowsUnavailableRpcException()
        {
            
            var request = new GetCitiesRequest();
            _mockRepo.Setup(r => r.GetCitiesAsync(request))
                .ThrowsAsync(new DatabaseException("Database offline."));

            var rpcException = await Assert.ThrowsAsync<RpcException>(() => _service.GetCities(request, null!));
            Assert.Equal(StatusCode.Unavailable, rpcException.StatusCode);
            Assert.Contains("Database offline.", rpcException.Status.Detail);
        }

        #endregion

        #region GetMakes Tests

        [Fact]
        public async Task GetMakes_Success_ReturnsGetMakesResponse()
        {
            var request = new GetMakesRequest();
            var makesList = new List<StocksAssignment.Grpc.Contracts.Make>
            {
                new StocksAssignment.Grpc.Contracts.Make { MakeId = 1, MakeName = "Maruti" },
                new StocksAssignment.Grpc.Contracts.Make { MakeId = 2, MakeName = "Hyundai" }
            };

            _mockRepo.Setup(r => r.GetMakesAsync(request))
                .ReturnsAsync(makesList);

            var response = await _service.GetMakes(request, null!);

            Assert.NotNull(response);
            Assert.Equal(2, response.Makes.Count);
            Assert.Equal("Maruti", response.Makes[0].MakeName);
            Assert.Equal("Hyundai", response.Makes[1].MakeName);
        }

        [Fact]
        public async Task GetMakes_DatabaseException_ThrowsUnavailableRpcException()
        {
            var request = new GetMakesRequest();
            _mockRepo.Setup(r => r.GetMakesAsync(request))
                .ThrowsAsync(new DatabaseException("DB error."));

            var rpcException = await Assert.ThrowsAsync<RpcException>(() => _service.GetMakes(request, null!));
            Assert.Equal(StatusCode.Unavailable, rpcException.StatusCode);
            Assert.Contains("DB error.", rpcException.Status.Detail);
        }

        #endregion
    }
}
