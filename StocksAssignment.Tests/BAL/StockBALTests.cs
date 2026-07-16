using Moq;
using StocksAssignment.BAL;
using StocksAssignment.Contracts;
using StocksAssignment.DAL;
using StocksAssignment.Domain.Entities;
using StocksAssignment.Mapper;

namespace StocksAssignment.Tests.BAL
{
    public class StockBALTests
    {
        private readonly Mock<IStockDAL> _mockDal;
        private readonly Mock<IStockMapper> _mockMapper;
        private readonly StockBAL _bal;

        public StockBALTests()
        {
            _mockDal = new Mock<IStockDAL>();
            _mockMapper = new Mock<IStockMapper>();
            _bal = new StockBAL(_mockDal.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetStocksAsync_ShouldCallDalAndMapperAndReturnDtoList()
        {
            var filters = new Filters();
            var stock1 = new Stock { Id = 1, Price = 180000, KilometersDriven = 5000 };
            var stock2 = new Stock { Id = 2, Price = 250000, KilometersDriven = 12000 };
            
            var domainStocks = new List<Stock> { stock1, stock2 };

            var dto1 = new StockDto { CarName = "2020 Make A" };
            var dto2 = new StockDto { CarName = "2021 Make B" };

            _mockDal.Setup(d => d.GetStocksAsync(filters))
                .ReturnsAsync(domainStocks);

            _mockMapper.Setup(m => m.ToStockDto(stock1))
                .Returns(dto1);

            _mockMapper.Setup(m => m.ToStockDto(stock2))
                .Returns(dto2);

            var result = await _bal.GetStocksAsync(filters);

            Assert.NotNull(result);
            Assert.Equal(2, result.Stocks.Count);
            Assert.Same(dto1, result.Stocks[0]);
            Assert.Same(dto2, result.Stocks[1]);
            Assert.False(result.HasNextPage);
            
            // Verify DAL was called once
            _mockDal.Verify(d => d.GetStocksAsync(filters), Times.Once);
        }

        [Fact]
        public async Task GetStocksAsync_WhenDalReturnsEmptyList_ShouldReturnEmptyList()
        {
            var filters = new Filters();
            var emptyList = new List<Stock>();

            _mockDal.Setup(d => d.GetStocksAsync(filters))
                .ReturnsAsync(emptyList);

            var result = await _bal.GetStocksAsync(filters);

            Assert.NotNull(result);
            Assert.Empty(result.Stocks);
            Assert.False(result.HasNextPage);

            // Verify DAL was called once
            _mockDal.Verify(d => d.GetStocksAsync(filters), Times.Once);
        }

        [Theory]
        // Boundary cases for KilometersDriven (Threshold: < 10000) and Price (Threshold: < 200000)

        [InlineData(5000, 150000, true)]

        [InlineData(9999, 150000, true)]   // Just below boundary
        [InlineData(10000, 150000, false)] // Exactly on boundary
        [InlineData(10001, 150000, false)] // Just above boundary

        [InlineData(5000, 199999, true)]   // Just below boundary
        [InlineData(5000, 200000, false)]  // Exactly on boundary
        [InlineData(5000, 200001, false)]  // Just above boundary

        [InlineData(9999, 199999, true)]   // Both just below boundary
        [InlineData(10000, 200000, false)] // Both exactly on boundary
        [InlineData(10001, 200001, false)] // Both just above boundary

        [InlineData(0, 0, true)]           // Zero values
        public async Task GetStocksAsync_ShouldEvaluateIsValueForMoneyCorrectly(int km, int price, bool expectedIsValueForMoney)
        {
            var filters = new Filters();
            var stock = new Stock 
            { 
                Id = 1, 
                KilometersDriven = km, 
                Price = price 
            };
            
            var domainStocks = new List<Stock> { stock };
            var dto = new StockDto { CarName = "Test Car" };

            _mockDal.Setup(d => d.GetStocksAsync(filters))
                .ReturnsAsync(domainStocks);

            _mockMapper.Setup(m => m.ToStockDto(stock))
                .Returns(dto);

            var result = await _bal.GetStocksAsync(filters);

            Assert.NotNull(result);
            Assert.Single(result.Stocks);
            Assert.Equal(expectedIsValueForMoney, result.Stocks[0].IsValueForMoney);
            Assert.False(result.HasNextPage);
        }

        [Fact]
        public async Task GetStocksAsync_WithMoreThanLimitResult_SetsHasNextPageTrueAndSlicesExtraItem()
        {
            var filters = new Filters { Limit = 2 };
            var stock1 = new Stock { Id = 1, Price = 100000 };
            var stock2 = new Stock { Id = 2, Price = 150000 };
            var stock3 = new Stock { Id = 3, Price = 200000 };
            var domainStocks = new List<Stock> { stock1, stock2, stock3 };

            var dto1 = new StockDto { CarName = "Make A" };
            var dto2 = new StockDto { CarName = "Make B" };

            _mockDal.Setup(d => d.GetStocksAsync(It.Is<Filters>(f => f.Limit == 3)))
                .ReturnsAsync(domainStocks);

            _mockMapper.Setup(m => m.ToStockDto(stock1)).Returns(dto1);
            _mockMapper.Setup(m => m.ToStockDto(stock2)).Returns(dto2);

            var result = await _bal.GetStocksAsync(filters);

            Assert.NotNull(result);
            Assert.True(result.HasNextPage);
            Assert.Equal(2, result.Stocks.Count);
            Assert.Same(dto1, result.Stocks[0]);
            Assert.Same(dto2, result.Stocks[1]);
        }

        [Fact]
        public async Task GetStocksAsync_WithLessThanOrEqualLimitResult_SetsHasNextPageFalseAndDoesNotSlice()
        {
            var filters = new Filters { Limit = 2 };
            var stock1 = new Stock { Id = 1, Price = 100000 };
            var domainStocks = new List<Stock> { stock1 };

            var dto1 = new StockDto { CarName = "Make A" };

            _mockDal.Setup(d => d.GetStocksAsync(It.Is<Filters>(f => f.Limit == 3)))
                .ReturnsAsync(domainStocks);

            _mockMapper.Setup(m => m.ToStockDto(stock1)).Returns(dto1);

            var result = await _bal.GetStocksAsync(filters);

            Assert.NotNull(result);
            Assert.False(result.HasNextPage);
            Assert.Single(result.Stocks);
            Assert.Same(dto1, result.Stocks[0]);
        }
    }
}
