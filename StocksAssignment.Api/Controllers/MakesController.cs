using Microsoft.AspNetCore.Mvc;
using StocksAssignment.BAL;

namespace StocksAssignment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakesController : ControllerBase
    {
        private readonly IStockBAL _stockBAL;

        public MakesController(IStockBAL stockBAL)
        {
            _stockBAL = stockBAL;
        }

        [HttpGet]
        public async Task<IActionResult> GetMakes()
        {
            var makes = await _stockBAL.GetMakesAsync();
            return Ok(makes);
        }
    }
}
