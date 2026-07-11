using Riok.Mapperly.Abstractions;
using StocksAssignment.Contracts;
using StocksAssignment.Domain.Entities;
using StocksAssignment.Domain.Enums;

namespace StocksAssignment.Mapper;

[Mapper]
public partial class StockMapper : IStockMapper
{
    [MapProperty(nameof(StockRequestDto.Fuel), nameof(Filters.FuelTypes), Use = nameof(MapFuelTypes))]
    [MapProperty(nameof(StockRequestDto.Car), nameof(Filters.MakeIds), Use = nameof(MapMakeIds))]
    [MapProperty(nameof(StockRequestDto.City), nameof(Filters.CityId))]
    [MapProperty(nameof(StockRequestDto.Budget), nameof(Filters.MinBudgetLakhs), Use = nameof(GetMinBudget))]
    [MapProperty(nameof(StockRequestDto.Budget), nameof(Filters.MaxBudgetLakhs), Use = nameof(GetMaxBudget))]
    public partial Filters ToFilters(StockRequestDto dto);

    [MapProperty(nameof(Stock.FuelType), nameof(StockDto.FuelType), Use = nameof(MapFuelType))]
    [MapProperty(nameof(Stock.Price), nameof(StockDto.FormattedPrice), Use = nameof(MapFormattedPrice))]
    [MapProperty(nameof(Stock), nameof(StockDto.CarName), Use = nameof(MapCarName))]
    [MapperIgnoreTarget(nameof(StockDto.IsValueForMoney))]
    public partial StockDto ToStockDto(Stock stock);

    private static List<FuelType> MapFuelTypes(string? fuel)
    {
        if (string.IsNullOrWhiteSpace(fuel))
            return [];

        return fuel.Split('+')
                   .Select(int.Parse)
                   .Select(id => (FuelType)id)
                   .ToList();
    }

    private static List<int> MapMakeIds(string? makes)
    {
        if (string.IsNullOrWhiteSpace(makes))
            return [];

        return makes.Split('+')
                    .Select(int.Parse)
                    .ToList();
    }

    private static int? GetMinBudget(string? budget)
    {
        if (string.IsNullOrWhiteSpace(budget))
            return null;

        return int.Parse(budget.Split('-')[0]) * 100000;
    }

    private static int? GetMaxBudget(string? budget)
    {
        if (string.IsNullOrWhiteSpace(budget))
            return null;

        var parts = budget.Split('-');

        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
            return null;

        return int.Parse(parts[1]) * 100000;
    }

    private static string MapFuelType(FuelType fuelType)
        => fuelType.ToString();

    private static string MapFormattedPrice(decimal price)
        => $"Rs. {price:0.##} Lakh";

    private static string MapCarName(Stock stock)
        => $"{stock.RegistrationYear} {stock.MakeName} {stock.ModelName}";
}