namespace MotorFinanceiro.API.Models
{
    public class Entrada
    {
        public Guid Id { get; set; }
        public decimal Valor { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; }
    }
}
