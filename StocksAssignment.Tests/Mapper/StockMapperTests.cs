using System;
using System.Collections.Generic;
using StocksAssignment.Contracts;
using StocksAssignment.Domain.Entities;
using StocksAssignment.Domain.Enums;
using StocksAssignment.Domain.Exceptions;
using StocksAssignment.Mapper;
using Xunit;

namespace StocksAssignment.Tests.Mapper
{
    public class StockMapperTests
    {
        private readonly StockMapper _mapper;

        public StockMapperTests()
        {
            _mapper = new StockMapper();
        }

        #region ToFilters Mapping Tests

        [Fact]
        public void ToFilters_WithNullOrEmptyDto_ReturnsFiltersWithDefaultValues()
        {
            var dto = new StockRequestDto
            {
                Fuel = null,
                Car = null,
                City = null,
                Budget = null
            };

            var result = _mapper.ToFilters(dto);

            Assert.NotNull(result);
            Assert.Empty(result.FuelTypes);
            Assert.Empty(result.MakeIds);
            Assert.Null(result.CityId);
            Assert.Null(result.MinBudgetLakhs);
            Assert.Null(result.MaxBudgetLakhs);
        }

        [Fact]
        public void ToFilters_WithValidSingleValues_MapsCorrectly()
        {
            var dto = new StockRequestDto
            {
                Fuel = "2", // Diesel
                Car = "10",
                City = 5,
                Budget = "12" // Min: 12L, Max: null
            };

            var result = _mapper.ToFilters(dto);

            Assert.NotNull(result);
            Assert.Single(result.FuelTypes, FuelType.Diesel);
            Assert.Single(result.MakeIds, 10);
            Assert.Equal(5, result.CityId);
            Assert.Equal(1200000, result.MinBudgetLakhs);
            Assert.Null(result.MaxBudgetLakhs);
        }

        [Fact]
        public void ToFilters_WithValidMultipleValuesAndRange_MapsCorrectly()
        {
            var dto = new StockRequestDto
            {
                Fuel = "1+2", // Diesel + Petrol
                Car = "10+20",
                City = 5,
                Budget = "5-15" // Min: 5L, Max: 15L
            };

            var result = _mapper.ToFilters(dto);

            Assert.NotNull(result);
            Assert.Equal(2, result.FuelTypes.Count);
            Assert.Contains(FuelType.Diesel, result.FuelTypes);
            Assert.Contains(FuelType.Petrol, result.FuelTypes);
            
            Assert.Equal(2, result.MakeIds.Count);
            Assert.Contains(10, result.MakeIds);
            Assert.Contains(20, result.MakeIds);

            Assert.Equal(5, result.CityId);
            Assert.Equal(500000, result.MinBudgetLakhs);
            Assert.Equal(1500000, result.MaxBudgetLakhs);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("1+abc")]
        [InlineData("1+")]
        public void ToFilters_WithInvalidFuel_ThrowsValidationException(string invalidFuel)
        {
            var dto = new StockRequestDto { Fuel = invalidFuel };

            var exception = Assert.Throws<ValidationException>(() => _mapper.ToFilters(dto));
            Assert.Contains("Invalid Fuel Type ID", exception.Message);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("10+abc")]
        [InlineData("10+")]
        public void ToFilters_WithInvalidCar_ThrowsValidationException(string invalidCar)
        {
            var dto = new StockRequestDto { Car = invalidCar };
            var exception = Assert.Throws<ValidationException>(() => _mapper.ToFilters(dto));
            Assert.Contains("Invalid Car/Make ID", exception.Message);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("5-abc")]
        [InlineData("abc-15")]
        public void ToFilters_WithInvalidBudget_ThrowsValidationException(string invalidBudget)
        {
            var dto = new StockRequestDto { Budget = invalidBudget };
            Assert.Throws<ValidationException>(() => _mapper.ToFilters(dto));
        }

        #endregion

        #region ToStockDto Mapping Tests

        [Fact]
        public void ToStockDto_WithValidStock_MapsAndFormatsCorrectly()
        {
            var stock = new Stock
            {
                Id = 100,
                MakeId = 2,
                MakeName = "Honda",
                ModelName = "Civic",
                RegistrationYear = 2020,
                FuelType = FuelType.Petrol,
                Price = 1250000, // 12.5 Lakhs
                KilometersDriven = 15000,
                CityId = 3
            };

            var result = _mapper.ToStockDto(stock);

            Assert.NotNull(result);
            Assert.Equal("Petrol", result.FuelType);
            Assert.Equal("Rs. 12.50 Lakh", result.FormattedPrice); // Maps using "0.00" formatting
            Assert.Equal(1250000, result.Price);
            Assert.Equal("2020 Honda Civic", result.CarName);
            // Verify source fields mapped correctly
            Assert.False(result.IsValueForMoney); // Should default to false initially, calculated by BAL
        }

        #endregion
    }
}
