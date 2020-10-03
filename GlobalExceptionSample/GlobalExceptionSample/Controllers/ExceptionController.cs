using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GlobalExceptionSample.Controllers
{
    [Route("api")]
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        [HttpGet("exception")]
        public async Task<ActionResult<string>> TestException()
        {
            await Task.Delay(1);

            //Ini akan menyebabkan exception
            int x = int.Parse("ini bukan angka");
            return Ok();
        }
    }
}
