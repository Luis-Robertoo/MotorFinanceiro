

using MotorFinanceiro.API.Interfaces;
using MotorFinanceiro.API.Models;
using System.Text.Json;

namespace MotorFinanceiro.API.Service
{
    public class CotacaoService : ICotacaoService
    {
        private readonly IConfiguration _configuration;
        public CotacaoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Valor>> ObtemCotacao(string moeda, string data = "atual") 
        {
            if (moeda.Length != 3 || moeda.Any(char.IsNumber))
                return null;

            var cliente = new HttpClient();

            data = data == "atual" ? DateTime.Now.ToString("MM-dd-yyyy") : data;

            var urlBase = _configuration.GetValue<string>("Urls:urlCotacao");

            var url = $"{urlBase}@codigoMoeda='{moeda.ToUpper()}'&@dataCotacao='{data}'&$format=json";

            var response = await cliente.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var dados = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Cotacao>(dados.Replace("@odata.context", "dataContexto")).value;
            
        }
    }
}
