using Microsoft.AspNetCore.Mvc;
using StocksAssignment.BAL;

namespace StocksAssignment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IStockBAL _stockBAL;

        public CitiesController(IStockBAL stockBAL)
        {
            _stockBAL = stockBAL;
        }

        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _stockBAL.GetCitiesAsync();
            return Ok(cities);
        }
    }
}
