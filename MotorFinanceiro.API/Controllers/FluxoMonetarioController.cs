using Microsoft.AspNetCore.Mvc;

namespace MotorFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FluxoMonetarioController : ControllerBase
    {
        [HttpGet(Name = "GetTest")]
        public async Task<ActionResult> Get([FromQuery] string palavra)
        {
            return Ok($"A palavra enviada foi {palavra}");
        }

    }
}
