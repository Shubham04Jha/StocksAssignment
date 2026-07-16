using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StocksAssignment.BAL;
using StocksAssignment.Contracts;
using StocksAssignment.Mapper;

namespace StocksAssignment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockBAL _stockBAL;
        private readonly IStockMapper _mapper;

        public StocksController(IStockBAL stockBAL, IStockMapper mapper)
        {
            _stockBAL = stockBAL;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks([FromQuery] StockRequestDto request)
        {
            var filters = _mapper.ToFilters(request);

            var currentPage = 1;
            if (!string.IsNullOrWhiteSpace(request.Pn) && int.TryParse(request.Pn, out var page))
            {
                currentPage = page;
            }

            var paginatedStocks = await _stockBAL.GetStocksAsync(filters);

            var response = new PaginatedStocksResponse
            {
                Stocks = paginatedStocks.Stocks,
                NextPageUrl = paginatedStocks.HasNextPage ? GetNextPageUrl(Request, currentPage) : null
            };

            return Ok(response);
        }

        private string GetNextPageUrl(Microsoft.AspNetCore.Http.HttpRequest req, int currentPage)
        {
            var queryParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in req.Query.Keys)
            {
                queryParams[key] = req.Query[key]!;
            }

            queryParams["pn"] = (currentPage + 1).ToString();

            var queryString = string.Join("&", queryParams.Select(kvp => $"{System.Uri.EscapeDataString(kvp.Key)}={System.Uri.EscapeDataString(kvp.Value)}"));
            return $"{req.Scheme}://{req.Host}{req.Path}?{queryString}";
        }
    }
}
