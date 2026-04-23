using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Models;
using HamburguerDev.Business.Notificacoes;

namespace HamburguerDev.Business.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly INotificador _notificador;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        INotificador notificador)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _notificador = notificador;
    }

    public async Task<IEnumerable<Pedido>> Buscar(int? codigo = null)
    {
        var pedidos = await _pedidoRepository.Buscar();

        if (codigo.HasValue)
        {
            pedidos = pedidos.Where(p => p.Codigo == codigo.Value);
        }

        return pedidos.OrderBy(p => p.DataCriacao);
    }

    public async Task<Pedido?> BuscarPorId(Guid id)
    {
        return await _pedidoRepository.BuscarPorId(id);
    }

    public async Task<PedidoValidacaoDTO?> ValidarPedido(IEnumerable<Guid>? produtosId)
    {
        var ids = produtosId?
            .Where(id => id != Guid.Empty)
            .ToList() ?? [];

        if (!ids.Any())
        {
            NotificarErro("Adicione ao menos um produto para validar o pedido.");
            return null;
        }

        var produtos = (await _produtoRepository.Buscar())
            .Where(p => ids.Contains(p.Id))
            .ToList();

        if (produtos.Count != ids.Count)
        {
            NotificarErro("Um ou mais produtos informados não foram encontrados.");
            return null;
        }

        var quantidadeSanduiches = produtos.Count(p => !p.Acompanhamento);
        if (quantidadeSanduiches > 1)
        {
            NotificarErro("O pedido pode conter apenas um sanduíche.");
        }

        var quantidadeBatatas = produtos.Count(p =>
            p.Acompanhamento &&
            p.Nome.Contains("batata", StringComparison.OrdinalIgnoreCase));

        if (quantidadeBatatas > 1)
        {
            NotificarErro("O pedido pode conter apenas uma batata.");
        }

        var quantidadeRefrigerantes = produtos.Count(p =>
            p.Acompanhamento &&
            p.Nome.Contains("refrigerante", StringComparison.OrdinalIgnoreCase));

        if (quantidadeRefrigerantes > 1)
        {
            NotificarErro("O pedido pode conter apenas um refrigerante.");
        }

        if (_notificador.TemNotificacao())
        {
            return null;
        }

        var subtotal = produtos.Sum(p => p.Preco);
        var temSanduiche = quantidadeSanduiches == 1;
        var temBatata = quantidadeBatatas == 1;
        var temRefrigerante = quantidadeRefrigerantes == 1;

        decimal descontoPorcentagem = 0m;

        if (temSanduiche && temBatata && temRefrigerante)
        {
            descontoPorcentagem = 20m;
        }
        else if (temSanduiche && temRefrigerante)
        {
            descontoPorcentagem = 15m;
        }
        else if (temSanduiche && temBatata)
        {
            descontoPorcentagem = 10m;
        }

        var valorDesconto = subtotal * (descontoPorcentagem / 100m);
        var totalComDesconto = subtotal - valorDesconto;

        return new PedidoValidacaoDTO
        {
            Subtotal = subtotal,
            Total = totalComDesconto,
            DescontoPorcentagem = descontoPorcentagem
        };
    }

    public async Task<Pedido?> InserirPedido(IEnumerable<Guid>? produtosId)
    {
        var validacao = await ValidarPedido(produtosId);
        if (validacao is null || _notificador.TemNotificacao())
        {
            return null;
        }

        var ids = produtosId?
            .Where(id => id != Guid.Empty)
            .ToList() ?? [];

        var proximoCodigo = await ObterProximoCodigo();
        var pedido = new Pedido(
            status: StatusPedidoEnum.CRIADO.ToString(),
            codigo: proximoCodigo,
            total: validacao.Total,
            subtotal: validacao.Subtotal,
            descontoPorcentagem: validacao.DescontoPorcentagem);

        var pedidoProdutos = ids
            .Select(id => new PedidoProduto(pedido.Id, id))
            .ToList();

        return await _pedidoRepository.Inserir(pedido, pedidoProdutos);
    }

    public async Task<Pedido?> FinalizarPedido(Guid id)
    {
        var pedido = await _pedidoRepository.BuscarPorId(id);

        if (pedido is null)
        {
            NotificarErro("Pedido não encontrado.");
            return null;
        }

        if (pedido.Status == StatusPedidoEnum.FINALIZADO.ToString())
        {
            NotificarErro("O pedido já está finalizado.");
            return null;
        }

        pedido.Finalizar();
        return await _pedidoRepository.Atualizar(pedido);
    }

    public async Task<bool> ExcluirPedido(Guid id)
    {
        var pedido = await _pedidoRepository.BuscarPorId(id);
        if (pedido is null)
        {
            NotificarErro("Pedido não encontrado.");
            return false;
        }

        var pedidoExcluido = await _pedidoRepository.Excluir(id);
        if (!pedidoExcluido)
        {
            NotificarErro("Erro ao excluir pedido.");
        }

        return pedidoExcluido;
    }

    public void Dispose()
    {
        _pedidoRepository.Dispose();
        _produtoRepository.Dispose();
    }

    private void NotificarErro(string mensagem)
    {
        _notificador.Handle(new Notificacao(mensagem));
    }

    private async Task<int> ObterProximoCodigo()
    {
        var pedidos = await _pedidoRepository.Buscar();
        if (!pedidos.Any())
        {
            return 1;
        }

        return pedidos.Max(p => p.Codigo) + 1;
    }
}
