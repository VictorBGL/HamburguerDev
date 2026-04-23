namespace HamburguerDev.Api.ViewModels
{
    public class PedidoDetalheResponseModel
    {
        public Guid? Id { get; set; }
        public string Status { get; set; }
        public int Codigo { get; set; }
        public decimal Total { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? DescontoPorcentagem { get; set; }

        public IEnumerable<PedidoProdutoResponseModel>? PedidoProdutos { get; set; }
    }
}
