namespace HamburguerDev.Api.ViewModels
{
    public class PedidoResponseModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public decimal Total { get; set; }
        public string Produtos { get; set; } = string.Empty;
    }
}
