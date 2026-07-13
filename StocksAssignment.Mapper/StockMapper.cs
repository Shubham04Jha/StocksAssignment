using Riok.Mapperly.Abstractions;
using StocksAssignment.Contracts;
using StocksAssignment.Domain.Entities;
using StocksAssignment.Domain.Enums;
using StocksAssignment.Domain.Exceptions;
using System.Numerics;

namespace StocksAssignment.Mapper;

[Mapper]
public partial class StockMapper : IStockMapper
{
    [MapProperty(nameof(StockRequestDto.Fuel), nameof(Filters.FuelTypes), Use = nameof(MapFuelTypes))]
    [MapProperty(nameof(StockRequestDto.Car), nameof(Filters.MakeIds), Use = nameof(MapMakeIds))]
    [MapProperty(nameof(StockRequestDto.City), nameof(Filters.CityId), Use = nameof(MapCityId))]
    [MapProperty(nameof(StockRequestDto.Budget), nameof(Filters.MinBudgetLakhs), Use = nameof(GetMinBudgetLakhs))]
    [MapProperty(nameof(StockRequestDto.Budget), nameof(Filters.MaxBudgetLakhs), Use = nameof(GetMaxBudgetLakhs))]
    [MapProperty(nameof(StockRequestDto.Sc), nameof(Filters.SortColumn), Use = nameof(MapSortColumn))]
    [MapProperty(nameof(StockRequestDto.So), nameof(Filters.SortOrder), Use = nameof(MapSortOrder))]
    public partial Filters ToFilters(StockRequestDto dto);

    [MapProperty(nameof(Stock.FuelType), nameof(StockDto.FuelType), Use = nameof(MapFuelType))]
    [MapProperty(nameof(Stock.Price), nameof(StockDto.FormattedPrice), Use = nameof(MapFormattedPrice))]
    [MapProperty(nameof(Stock), nameof(StockDto.CarName), Use = nameof(MapCarName))]
    [MapperIgnoreTarget(nameof(StockDto.IsValueForMoney))]
    public partial StockDto ToStockDto(Stock stock);

    public partial CityDto ToCityDto(City city);

    public partial MakeDto ToMakeDto(Make make);

    private static List<FuelType> MapFuelTypes(string? fuel)
    {
        if (string.IsNullOrWhiteSpace(fuel))
            return [];

        var list = new List<FuelType>();
        foreach (var part in fuel.Split(new[]{'+',' '}, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!int.TryParse(part, out var id))
            {
                throw new ValidationException($"Invalid Fuel Type ID: '{part}'. Fuel parameter must be a list of integers separated by '+'.");
            }
            if (!Enum.IsDefined(typeof(FuelType), id))
            {
                throw new ValidationException($"Invalid Fuel Type ID: '{id}'. Supported values are: 1 (Petrol), 2 (Diesel), 3 (CNG), 4 (LPG), 5 (Electric), 6 (Hybrid).");
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
        foreach (var part in makes.Split(new[]{'+',' '}, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!int.TryParse(part, out var id))
            {
                throw new ValidationException($"Invalid Car/Make ID: '{part}'. Car parameter must be a list of integers separated by '+'.");
            }
            list.Add(id);
        }
        return list;
    }

    private static int? MapCityId(string? city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return null;

        if (!int.TryParse(city, out var id))
        {
            if (BigInteger.TryParse(city, out _))
            {
                throw new ValidationException($"City ID: '{city}' is out of range. Value must be between 0 and {int.MaxValue}.");
            }
            throw new ValidationException($"Invalid City ID: '{city}'. City parameter must be an integer.");
        }

        if (id < 0)
        {
            throw new ValidationException($"City ID: '{city}' must be a non-negative integer.");
        }

        return id;
    }

    private static int? GetMinBudgetLakhs(string? budget)
    {
        if (string.IsNullOrWhiteSpace(budget))
            return null;

        string minStr;
        if (budget.StartsWith("-"))
        {
            var parts = budget.Split('-');
            minStr = "-" + parts[1];
        }
        else
        {
            minStr = budget.Split('-')[0];
        }

        if (string.IsNullOrWhiteSpace(minStr))
            return null;

        if (!int.TryParse(minStr, out var value))
        {
            if (BigInteger.TryParse(minStr, out _))
            {
                throw new ValidationException($"Minimum budget value: '{minStr}' is out of range. Value must be between 0 and {int.MaxValue}.");
            }
            throw new ValidationException($"Invalid minimum budget value: '{minStr}'. Budget parameter must be a numeric value or range (e.g., '10' or '5-15').");
        }

        if (value < 0)
        {
            throw new ValidationException($"Minimum budget value: '{minStr}' cannot be negative.");
        }
        return value;
    }

    private static int? GetMaxBudgetLakhs(string? budget)
    {
        if (string.IsNullOrWhiteSpace(budget))
            return null;

        int sepIndex = budget.IndexOf('-', budget.StartsWith("-") ? 2 : 1);
        if (sepIndex == -1)
            return null;

        var maxStr = budget.Substring(sepIndex + 1);
        if (string.IsNullOrWhiteSpace(maxStr))
            return null;

        if (!int.TryParse(maxStr, out var value))
        {
            if (BigInteger.TryParse(maxStr, out _))
            {
                throw new ValidationException($"Maximum budget value: '{maxStr}' is out of range. Value must be between 0 and {int.MaxValue}.");
            }
            throw new ValidationException($"Invalid maximum budget value: '{maxStr}'. Budget parameter must be a numeric value or range (e.g., '10' or '5-15').");
        }

        if (value < 0)
        {
            throw new ValidationException($"Maximum budget value: '{maxStr}' cannot be negative.");
        }
        return value;
    }

    private static SortColumn MapSortColumn(string? sc)
    {
        if (string.IsNullOrWhiteSpace(sc))
            return SortColumn.Price;

        if (!int.TryParse(sc, out var val))
        {
            throw new ValidationException($"Invalid sort column: '{sc}'. Supported values are: 1 (Price), 2 (KilometersDriven), 3 (RegistrationYear).");
        }

        if (Enum.IsDefined(typeof(SortColumn), val))
        {
            return (SortColumn)val;
        }

        throw new ValidationException($"Invalid sort column: '{val}'. Supported values are: 1 (Price), 2 (KilometersDriven), 3 (RegistrationYear).");
    }

    private static SortOrder MapSortOrder(string? so)
    {
        if (string.IsNullOrWhiteSpace(so))
            return SortOrder.Ascending;

        if (!int.TryParse(so, out var val))
        {
            throw new ValidationException($"Invalid sort order: '{so}'. Supported values are: 1 (Ascending), 0 (Descending).");
        }

        if (Enum.IsDefined(typeof(SortOrder), val))
        {
            return (SortOrder)val;
        }

        throw new ValidationException($"Invalid sort order: '{val}'. Supported values are: 1 (Ascending), 0 (Descending).");
    }

    private static string MapFuelType(FuelType fuelType)
        => fuelType.ToString();

    private static string MapFormattedPrice(long price)
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