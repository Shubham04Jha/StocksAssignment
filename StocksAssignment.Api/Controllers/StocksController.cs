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

            var stocks = await _stockBAL.GetStocksAsync(filters);

            return Ok(stocks);
        }
    }
}
