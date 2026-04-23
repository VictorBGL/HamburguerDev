using HamburguerDev.Business.Models;

namespace HamburguerDev.Api.ViewModels
{
    public class PedidoProdutoResponseModel
    {
        public Guid Id { get; set; }
        public Guid PedidoId { get; set; }
        public Guid ProdutoId { get; set; }

        public ProdutoResponseModel Produto { get; set; }
    }
}
