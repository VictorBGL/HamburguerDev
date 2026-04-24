using HamburguerDev.Frontend.Models;

namespace HamburguerDev.Frontend.Services;

public interface IPedidoApiService
{
    Task<(IReadOnlyList<PedidoResponseModel> Pedidos, IReadOnlyList<string> Errors)> BuscarPedidosAsync(int? codigo, CancellationToken cancellationToken = default);
    Task<(PedidoDetalheResponseModel? Pedido, IReadOnlyList<string> Errors)> BuscarPedidoPorIdAsync(Guid pedidoId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ProdutoResponseModel> Produtos, IReadOnlyList<string> Errors)> BuscarProdutosAsync(string? nome = null, int? codigo = null, CancellationToken cancellationToken = default);
    Task<(PedidoValidacaoDto? Validacao, IReadOnlyList<string> Errors)> ValidarPedidoAsync(IEnumerable<Guid> produtosId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> InserirPedidoAsync(IEnumerable<Guid> produtosId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> AtualizarPedidoAsync(Guid pedidoId, IEnumerable<Guid> produtosId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> FinalizarPedidoAsync(Guid pedidoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ExcluirPedidoAsync(Guid pedidoId, CancellationToken cancellationToken = default);
}
