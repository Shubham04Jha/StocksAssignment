using Microsoft.AspNetCore.Mvc;
using StocksAssignment.Grpc.Contracts;

namespace StocksAssignment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly Stocks.StocksClient _client;

        public TestController(Stocks.StocksClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _client.GetStocksAsync(new GetStocksRequest());

            return Ok(response);
        }
    }
}