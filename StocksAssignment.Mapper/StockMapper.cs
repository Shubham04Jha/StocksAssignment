using Riok.Mapperly.Abstractions;
using StocksAssignment.Contracts;
using StocksAssignment.Domain.Entities;
using StocksAssignment.Domain.Enums;
using StocksAssignment.Domain.Exceptions;

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

        var list = new List<FuelType>();
        foreach (var part in fuel.Split('+'))
        {
            if (!int.TryParse(part, out var id))
            {
                throw new ValidationException($"Invalid Fuel Type ID: '{part}'. Fuel parameter must be a list of integers separated by '+'.");
            }
            list.Add((FuelType)id);
        }
        return list;
    }

    private static List<int> MapMakeIds(string? makes)
    {
        if (string.IsNullOrWhiteSpace(makes))
            return [];

        var list = new List<int>();
        foreach (var part in makes.Split('+'))
        {
            if (!int.TryParse(part, out var id))
            {
                throw new ValidationException($"Invalid Car/Make ID: '{part}'. Car parameter must be a list of integers separated by '+'.");
            }
            list.Add(id);
        }
        return list;
    }

    private static int? GetMinBudget(string? budget)
    {
        if (string.IsNullOrWhiteSpace(budget))
            return null;

        var part = budget.Split('-')[0];
        if (!int.TryParse(part, out var value))
        {
            throw new ValidationException($"Invalid minimum budget value: '{part}'. Budget parameter must be a numeric value or range (e.g., '10' or '5-15').");
        }
        return value * 100000;
    }

    private static int? GetMaxBudget(string? budget)
    {
        if (string.IsNullOrWhiteSpace(budget))
            return null;

        var parts = budget.Split('-');

        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
            return null;

        var part = parts[1];
        if (!int.TryParse(part, out var value))
        {
            throw new ValidationException($"Invalid maximum budget value: '{part}'. Budget parameter must be a numeric value or range (e.g., '10' or '5-15').");
        }
        return value * 100000;
    }

    private static string MapFuelType(FuelType fuelType)
        => fuelType.ToString();

    private static string MapFormattedPrice(int price)
    {
        if (price < 100000)
        {
            return $"Rs. {price}";
        }

        var roundedLakhs = Math.Ceiling(price / 1000.0) / 100.0;
        if (roundedLakhs < 100.0)
        {
            return $"Rs. {roundedLakhs:0.00} Lakh";
        }
        else
        {
            var roundedCrores = Math.Ceiling(price / 100000.0) / 100.0;
            return $"Rs. {roundedCrores:0.00} Crore";
        }
    }

    private static string MapCarName(Stock stock)
        => $"{stock.RegistrationYear} {stock.MakeName} {stock.ModelName}";
}