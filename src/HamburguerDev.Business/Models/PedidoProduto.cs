
namespace HamburguerDev.Business.Models
{
    public class PedidoProduto : Entity
    {
        private PedidoProduto()
        {
            Pedido = null!;
            Produto = null!;
        }

        public PedidoProduto(Guid pedidoId, Guid produtoId)
        {
            PedidoId = pedidoId;
            ProdutoId = produtoId;
            Pedido = null!;
            Produto = null!;
        }

        public Guid PedidoId { get; private set; }
        public Guid ProdutoId { get; private set; }

        public virtual Pedido Pedido { get; private set; }
        public virtual Produto Produto { get; private set; }
    }
}
