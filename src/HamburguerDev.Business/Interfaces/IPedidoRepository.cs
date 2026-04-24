using HamburguerDev.Business.Models;

namespace HamburguerDev.Business.Interfaces;

public interface IPedidoRepository : IDisposable
{
    Task<IEnumerable<Pedido>> Buscar();
    Task<Pedido?> BuscarPorId(Guid id);
    Task<Pedido?> Inserir(Pedido pedido, IEnumerable<PedidoProduto> pedidoProdutos);
    Task<Pedido?> Atualizar(Pedido pedido);
    Task<Pedido?> Atualizar(Pedido pedido, IEnumerable<PedidoProduto> pedidoProdutos);
    Task<bool> Excluir(Guid id);
}
