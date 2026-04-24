namespace HamburguerDev.Frontend.Models;

public class PedidoProdutoResponseModel
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public ProdutoResponseModel Produto { get; set; } = new();
}
