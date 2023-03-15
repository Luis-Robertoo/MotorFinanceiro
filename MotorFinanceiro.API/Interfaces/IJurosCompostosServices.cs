using System.Net.NetworkInformation;

namespace MotorFinanceiro.API.Interfaces
{
    public interface IJurosCompostosServices
    {
        Task<List<string>> CalculaJurosSelic(string valorInicial, string valorMensal, int meses = 0);
    }
}
