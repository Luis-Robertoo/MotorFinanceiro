using MotorFinanceiro.API.Models;

namespace MotorFinanceiro.API.Interfaces
{
    public interface ICotacaoService
    {
        Task<List<Valor>> ObtemCotacao(string moeda, string data = "");
       
    }
}
