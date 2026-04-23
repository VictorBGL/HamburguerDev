namespace HamburguerDev.Business.Models
{
    public class PedidoValidacaoDTO
    {
        public decimal Total { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? DescontoPorcentagem { get; set; }
    }
}
