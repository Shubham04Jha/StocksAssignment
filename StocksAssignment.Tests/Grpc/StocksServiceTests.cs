extern alias GrpcServer;

using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using StocksAssignment.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using StocksService = GrpcServer::StocksAssignment.Grpc.Services.StocksService;
using IStockRepository = GrpcServer::StocksAssignment.GrpcServer.Repositories.IStockRepository;
using GetStocksRequest = GrpcServer::StocksAssignment.Grpc.Contracts.GetStocksRequest;
using GetStocksResponse = GrpcServer::StocksAssignment.Grpc.Contracts.GetStocksResponse;
using GrpcStock = GrpcServer::StocksAssignment.Grpc.Contracts.Stock;
using GetCitiesRequest = GrpcServer::StocksAssignment.Grpc.Contracts.GetCitiesRequest;
using GetCitiesResponse = GrpcServer::StocksAssignment.Grpc.Contracts.GetCitiesResponse;
using GrpcCity = GrpcServer::StocksAssignment.Grpc.Contracts.City;
using GetMakesRequest = GrpcServer::StocksAssignment.Grpc.Contracts.GetMakesRequest;
using GetMakesResponse = GrpcServer::StocksAssignment.Grpc.Contracts.GetMakesResponse;
using GrpcMake = GrpcServer::StocksAssignment.Grpc.Contracts.Make;

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
            var stock1 = new GrpcStock { StockId = 1, MakeName = "Honda", ModelName = "Civic", Price = 800000 };
            stock1.ImageUrls.Add("http://example.com/honda1.jpg");
            stock1.ImageUrls.Add("http://example.com/honda2.jpg");

            var stock2 = new GrpcStock { StockId = 2, MakeName = "Toyota", ModelName = "Corolla", Price = 900000 };

            var stocksList = new List<GrpcStock> { stock1, stock2 };

            _mockRepo.Setup(r => r.GetStocksAsync(request))
                .ReturnsAsync(stocksList);

            var response = await _service.GetStocks(request, null!);

            Assert.NotNull(response);
            Assert.Equal(2, response.Stocks.Count);
            Assert.Equal("Honda", response.Stocks[0].MakeName);
            Assert.Equal(2, response.Stocks[0].ImageUrls.Count);
            Assert.Equal("http://example.com/honda1.jpg", response.Stocks[0].ImageUrls[0]);
            Assert.Equal("Toyota", response.Stocks[1].MakeName);
            Assert.Empty(response.Stocks[1].ImageUrls);
        }

        [Fact]
        public async Task GetStocks_DatabaseException_ThrowsUnavailableRpcException()
        {
            var request = new GetStocksRequest();
            _mockRepo.Setup(r => r.GetStocksAsync(request))
                .ThrowsAsync(new DatabaseException("Database connection failure.", new Exception("DB is offline")));

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
            var citiesList = new List<GrpcCity>
            {
                new GrpcCity { CityId = 1, CityName = "Delhi" },
                new GrpcCity { CityId = 2, CityName = "Mumbai" }
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
                .ThrowsAsync(new DatabaseException("Database offline.", new Exception("DB unreachable")));

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
            var makesList = new List<GrpcMake>
            {
                new GrpcMake { MakeId = 1, MakeName = "Maruti" },
                new GrpcMake { MakeId = 2, MakeName = "Hyundai" }
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
                .ThrowsAsync(new DatabaseException("DB error.", new Exception("DB execution timed out")));

            var rpcException = await Assert.ThrowsAsync<RpcException>(() => _service.GetMakes(request, null!));
            Assert.Equal(StatusCode.Unavailable, rpcException.StatusCode);
            Assert.Contains("DB error.", rpcException.Status.Detail);
        }

        #endregion
    }
}
