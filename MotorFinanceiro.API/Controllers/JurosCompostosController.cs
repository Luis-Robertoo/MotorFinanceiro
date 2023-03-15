using Microsoft.AspNetCore.Mvc;
using MotorFinanceiro.API.Interfaces;
using MotorFinanceiro.API.Service;

namespace MotorFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JurosCompostosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJurosCompostosServices _jurosServices;

        public JurosCompostosController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jurosServices = new JurosCompostosService(_configuration);
        }

        [HttpGet(Name = "JurosCompostos")]
        public async Task<ActionResult> GetJurosCompostos([FromQuery] string? aporteInicial = "0", [FromQuery] string? aporteMensal = "0", [FromQuery] int meses = 12)
        {
            var dados = await _jurosServices.CalculaJurosSelic(aporteInicial, aporteMensal, meses);

            if (dados is not null)
                return Ok(dados);
            
            return BadRequest("Necessario valor maior que 0 no aporte inicial ou aporte mensal");
        }
    }
}
