using Microsoft.AspNetCore.Mvc;
using MotorFinanceiro.API.Interfaces;
using MotorFinanceiro.API.Models;
using MotorFinanceiro.API.Service;
using System.Text.Json;

namespace MotorFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotacaoController : ControllerBase
    {
        private readonly ICotacaoService _cotacaoService;
        private readonly IConfiguration _configuration;

        public CotacaoController(IConfiguration configuration)
        {
            _configuration = configuration;
            _cotacaoService = new CotacaoService(_configuration);
        }


        [HttpGet(Name = "Cotacao")]
        public async Task<ActionResult> GetCotacao([FromQuery] string moeda, [FromQuery] string data =  "atual")
        {

            var dados = await _cotacaoService.ObtemCotacao(moeda, data);

            if(dados != null)
                return Ok(dados);

            return BadRequest("O campo moeda é a sigla. Ex: USD para Dolar ou EUR para Euro");
            
        }
    }
}
