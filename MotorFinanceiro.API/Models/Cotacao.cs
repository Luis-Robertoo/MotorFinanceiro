using System.Xml.Linq;

namespace MotorFinanceiro.API.Models
{
    public class Cotacao
    {
        public string dataContexto { get; set; }
        public List<Valor> value { get; set; }

    }

    public class Valor
    {
        public decimal cotacaoCompra { get; set; }
        public decimal cotacaoVenda { get; set; }
        public string dataHoraCotacao { get; set; }
        public string tipoBoletim { get; set; }
    }
}
