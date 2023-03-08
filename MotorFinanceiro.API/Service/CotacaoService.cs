using MotorFinanceiro.API.Interfaces;
using MotorFinanceiro.API.Models;
using System.Text.Json;

namespace MotorFinanceiro.API.Service
{
    public class CotacaoService : ICotacaoService
    {
        public async Task<List<Valor>> ObtemCotacao(string moeda, string data = "atual") 
        {
            if (moeda.Length != 3 || moeda.Any(char.IsNumber))
                return null;

            var cliente = new HttpClient();

            data = data == "atual" ? DateTime.Now.ToString("MM-dd-yyyy") : data;

            var url = $"https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/CotacaoMoedaAberturaOuIntermediario(codigoMoeda=@codigoMoeda,dataCotacao=@dataCotacao)?@codigoMoeda='{moeda.ToUpper()}'&@dataCotacao='{data}'&$format=json";

            var response = await cliente.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var dados = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Cotacao>(dados.Replace("@odata.context", "dataContexto")).value;
            
        }
    }
}
