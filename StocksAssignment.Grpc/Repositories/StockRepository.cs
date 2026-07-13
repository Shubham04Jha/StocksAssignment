using Dapper;
using MySqlConnector;
using StocksAssignment.Grpc.Contracts;
using System;
using System.Data;
using System.Text;
using StocksAssignment.Domain.Exceptions;

namespace StocksAssignment.GrpcServer.Repositories
{
    public class StockRepository: IStockRepository
    {
        private readonly string _connectionString;
        public StockRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Configuration value 'DefaultConnection' is missing or empty.");
            }
            _connectionString = connectionString;
        }
        private IDbConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<List<Stock>> GetStocksAsync(GetStocksRequest request)
        {
            var queryBuilder = new StringBuilder(@"
                SELECT 
                    s.StockId, 
                    s.MakeId, 
                    m.MakeName, 
                    s.ModelName, 
                    s.RegistrationYear, 
                    s.FuelType, 
                    s.Price, 
                    s.KilometersDriven, 
                    s.CityId
                FROM Stocks s
                INNER JOIN Makes m ON s.MakeId = m.MakeId
                WHERE 1 = 1");
            var parameters = new DynamicParameters();
            
            if (request.FuelTypes.Count != 0)
            {
                queryBuilder.Append(" AND s.FuelType IN @FuelTypes");
                parameters.Add("FuelTypes", request.FuelTypes.ToList());
            }
            
            if (request.MakeIds.Count != 0)
            {
                queryBuilder.Append(" AND s.MakeId IN @MakeIds");
                parameters.Add("MakeIds", request.MakeIds.ToList());
            }
            
            if (request.HasCityId)
            {
                queryBuilder.Append(" AND s.CityId = @CityId");
                parameters.Add("CityId", request.CityId);
            }
            
            if (request.HasMinBudgetLakhs)
            {
                queryBuilder.Append(" AND s.Price >= @MinBudget");
                parameters.Add("MinBudget", request.MinBudgetLakhs);
            }
            
            if (request.HasMaxBudgetLakhs)
            {
                queryBuilder.Append(" AND s.Price <= @MaxBudget");
                parameters.Add("MaxBudget", request.MaxBudgetLakhs);
            }

            var sortColumn = request.HasSc ? request.Sc : 1;
            var sortOrder = request.HasSo ? request.So : 1;

            var sortColumnStr = sortColumn switch
            {
                2 => "s.KilometersDriven",
                3 => "s.RegistrationYear",
                _ => "s.Price"
            };

            var sortOrderStr = sortOrder switch
            {
                0 => "DESC",
                _ => "ASC"
            };

            queryBuilder.Append($" ORDER BY {sortColumnStr} {sortOrderStr}");

            try
            {
                using var connection = CreateConnection();
                var result = await connection.QueryAsync<Stock>(queryBuilder.ToString(), parameters);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Failed to query the database server.", ex);
            }
        }
    }
}
