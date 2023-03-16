using MotorFinanceiro.API.Interfaces;
using MotorFinanceiro.API.Models;
using System.Text.Json;

namespace MotorFinanceiro.API.Service
{
    public class JurosCompostosService : IJurosCompostosServices
    {
        private readonly IConfiguration _configuration;

        public JurosCompostosService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> CalculaJurosSelic(string valorInicialString = "0", string valorMensalString = "0", int meses = 0)
        {

            decimal valorInicial = Convert.ToDecimal(valorInicialString);
            decimal valorMensal = Convert.ToDecimal(valorMensalString); 

            if (valorInicial == 0 && valorMensal == 0)
                return null;

            var selic = await PegaSelic();

            if (selic == 0)
                return null;


            decimal aporteAcumulado = 0;
            decimal taxaJurosMensal = selic / 12;
            decimal porc = taxaJurosMensal / 100;
            decimal totalSemJuros = (meses * valorMensal) + valorInicial;

            List<string> resultado = new List<string>();
            
            for (int i = 1; i <= meses; i++)
            {
                //taxa / 100 = porcetagem // 2 / 100 = 0,02
                //porcetagem x numero = rendimento // 0,02 * 1000 = 20
                decimal rendiMensal = porc * valorInicial;

                if (i == 1)
                    aporteAcumulado = (porc * valorInicial) + valorInicial + valorMensal;
                else
                {
                    rendiMensal = porc * aporteAcumulado;
                    aporteAcumulado = aporteAcumulado + valorMensal + rendiMensal;
                }

                resultado.Add($"Mês - {i:D3} || Rendimento: {rendiMensal:0000.00} || Valor Acumulado: {aporteAcumulado:00000.00}");
            }

            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Taxa Selic: {selic}% ao ano || Taxa Selic: {taxaJurosMensal}% ao mês ");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total que saiu do seu bolso: {totalSemJuros}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total acumulado: {aporteAcumulado:00000.00}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Sendo {meses} de {valorMensal} mais 1 de {valorInicial}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total que você ganhou sem fazer nada: {aporteAcumulado - totalSemJuros:.00}");

            return resultado;
        }

        private async Task<decimal> PegaSelic()
        {
            var urlSelic = _configuration.GetValue<string>("Urls:urlSelic");
            
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Host", "api.bcb.gov.br");
            client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.31.1");

            var response = await client.GetAsync(urlSelic);

            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && json.Contains("valor"))
                return ConverteJsonToDouble(json);

            return 0;
        }

        private static decimal ConverteJsonToDouble(string json)
        {
            var dados = JsonSerializer.Deserialize<List<SelicResponse>>(json);

            Console.WriteLine(json);

            Console.WriteLine(dados.FirstOrDefault().valor);
            Console.WriteLine(dados.FirstOrDefault().valor.Replace(".", ","));

            Console.WriteLine(Convert.ToDecimal(dados.FirstOrDefault().valor));
            Console.WriteLine(Convert.ToDecimal(dados.FirstOrDefault().valor.Replace(".", ",")));
            

            return Convert.ToDecimal(dados.FirstOrDefault().valor.Replace(".", ","));
        }
    }
}
