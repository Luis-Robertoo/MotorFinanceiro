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

            var valorInicial = Convert.ToDecimal(valorInicialString);
            var valorMensal = Convert.ToDecimal(valorMensalString);

            if (valorInicial == 0 && valorMensal == 0)
                throw new Exception("Necessario valor maior que 0 no aporte inicial ou aporte mensal.");

            var selic = await PegaSelic();

            if (selic == 0)
                return null;


            var aporteAcumulado = decimal.Zero;
            var taxaJurosMensal = selic / 12;
            var porc = taxaJurosMensal / 100;
            var totalSemJuros = (meses * valorMensal) + valorInicial;
            var imposto = 22m / 100m;

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

                resultado.Add($"Mês - {i:D3} || Rend. Bruto: {rendiMensal:0000.00} || Rend. Liquido: {rendiMensal - (imposto * rendiMensal):0000.00} || Valor Acumulado: {aporteAcumulado:00000.00}");
            }

            var totalBruto = aporteAcumulado - totalSemJuros;
            var totalLiquido = totalBruto - imposto * totalBruto;

            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Taxa Selic: {selic}% ao ano || Taxa Selic: {taxaJurosMensal}% ao mês ");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total que saiu do seu bolso: {totalSemJuros}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total BRUTO acumulado: {aporteAcumulado:00000.00}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total Liquido acumulado: {totalSemJuros + totalLiquido:00000.00}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Sendo {meses} de {valorMensal} mais 1 de {valorInicial}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total que você ganhou sem fazer nada BRUTO: {aporteAcumulado - totalSemJuros:.00}");
            resultado.Add($"-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=--=-=-=-=-=-=-=-=-=-=");
            resultado.Add($"Total que você ganhou sem fazer nada LIQUIDO: {totalLiquido:.00}");

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

            return decimal.Zero;
        }

        private static decimal ConverteJsonToDouble(string json)
        {
            var dados = JsonSerializer.Deserialize<List<SelicResponse>>(json);

            return Convert.ToDecimal(dados.FirstOrDefault().valor.Replace(".", ","));
        }
    }
}
