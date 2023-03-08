using Microsoft.AspNetCore.Mvc;
using MotorFinanceiro.API.Models;
using System.Text.Json;

namespace MotorFinanceiro.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CotacaoController : ControllerBase
    {
        private readonly HttpClient cliente;

        public CotacaoController()
        {
            cliente = new HttpClient();
        }


        [HttpGet(Name = "Cotacao")]
        public async Task<ActionResult> GetCotacao([FromQuery] string moeda)
        {
            if (moeda.Length != 3 || moeda.Any(char.IsNumber))
                return BadRequest("O campo moeda é a sigla. EX: USD para Dolar ou EUR para Euro");

            var data = DateTime.Now.ToString("MM-dd-yyyy");
            var url = $"https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/CotacaoMoedaAberturaOuIntermediario(codigoMoeda=@codigoMoeda,dataCotacao=@dataCotacao)?@codigoMoeda='{moeda.ToUpper()}'&@dataCotacao='{data}'&$format=json";

            var response = await cliente.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                

                var dados = await response.Content.ReadAsStringAsync();
                dados = dados.Replace("@odata.context", "dataContexto");
                
                var json = JsonSerializer.Deserialize<Cotacao>(dados);

                return Ok(json);
            }

            return BadRequest(response.Content.ReadAsStringAsync());
            
        }
    }
}
