using StocksAssignment.BAL;
using StocksAssignment.DAL;
using StocksAssignment.Grpc.Contracts;
using StocksAssignment.Mapper;
using StocksAssignment.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddGrpcClient<Stocks.StocksClient>(options =>
{
    var serverUrl = builder.Configuration["GrpcSettings:ServerUrl"];

    if (string.IsNullOrWhiteSpace(serverUrl))
    {
        throw new InvalidOperationException(
            "Configuration value 'GrpcSettings:ServerUrl' is missing or empty.");
    }
    options.Address = new Uri(serverUrl);
});

builder.Services.AddScoped<IStockDAL, StockDAL>();

builder.Services.AddSingleton<IStockMapper, StockMapper>();

builder.Services.AddScoped<IStockBAL,StockBAL>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
