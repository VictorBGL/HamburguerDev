using HamburguerDev.Business.Models;

namespace HamburguerDev.Business.Interfaces;

public interface IPedidoService : IDisposable
{
    Task<IEnumerable<Pedido>> Buscar(int? codigo = null);
    Task<Pedido?> BuscarPorId(Guid id);
    Task<PedidoValidacaoDTO?> ValidarPedido(IEnumerable<Guid>? produtosId);
    Task<Pedido?> InserirPedido(IEnumerable<Guid>? produtosId);
    Task<Pedido?> AtualizarPedido(Guid id, IEnumerable<Guid>? produtosId);
    Task<Pedido?> FinalizarPedido(Guid id);
    Task<bool> ExcluirPedido(Guid id);
}
