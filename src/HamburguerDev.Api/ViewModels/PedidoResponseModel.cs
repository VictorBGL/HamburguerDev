namespace HamburguerDev.Api.ViewModels
{
    public class PedidoResponseModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public int Codigo { get; set; }
        public decimal Total { get; set; }
    }
}
