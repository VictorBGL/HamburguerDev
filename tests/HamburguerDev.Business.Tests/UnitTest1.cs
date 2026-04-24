using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Models;
using HamburguerDev.Business.Notificacoes;
using HamburguerDev.Business.Services;
using Moq;

namespace HamburguerDev.Business.Tests;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Notificador _notificador;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _notificador = new Notificador();
        _service = new PedidoService(_pedidoRepositoryMock.Object, _produtoRepositoryMock.Object, _notificador);
    }

    [Fact]
    public async Task Buscar_DeveFiltrarPorCodigo()
    {
        var pedido1 = CriarPedido(codigo: 1);
        var pedido2 = CriarPedido(codigo: 2);

        _pedidoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Pedido> { pedido1, pedido2 });

        var resultado = await _service.Buscar(2);

        Assert.Single(resultado);
        Assert.Equal(2, resultado.First().Codigo);
    }

    [Fact]
    public async Task BuscarPorId_DeveRetornarPedidoDoRepositorio()
    {
        var pedidoId = Guid.NewGuid();
        var pedido = CriarPedido(codigo: 10);

        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(pedidoId))
            .ReturnsAsync(pedido);

        var resultado = await _service.BuscarPorId(pedidoId);

        Assert.NotNull(resultado);
        Assert.Equal(pedido.Id, resultado!.Id);
    }

    [Fact]
    public async Task ValidarPedido_DeveNotificarErro_QuandoNaoHaProdutos()
    {
        var resultado = await _service.ValidarPedido([]);

        Assert.Null(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Adicione ao menos um produto para validar o pedido.");
    }

    [Fact]
    public async Task ValidarPedido_DeveNotificarErro_QuandoProdutoNaoExiste()
    {
        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>());

        var resultado = await _service.ValidarPedido([Guid.NewGuid()]);

        Assert.Null(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Um ou mais produtos informados não foram encontrados.");
    }

    [Fact]
    public async Task ValidarPedido_DeveNotificarBatataDuplicada_QuandoMesmoIdEnviadoDuasVezes()
    {
        var batataId = Guid.NewGuid();
        var batata = CriarProduto("Batata frita", 10m, 2, true, batataId);

        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto> { batata });

        var resultado = await _service.ValidarPedido([batataId, batataId]);

        Assert.Null(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "O pedido pode conter apenas uma batata.");
        Assert.DoesNotContain(_notificador.ObterNotificacoes(), n => n.Mensagem == "Um ou mais produtos informados não foram encontrados.");
    }

    [Fact]
    public async Task ValidarPedido_DeveAplicarDescontoDe20PorCento_QuandoTemComboCompleto()
    {
        var sanduicheId = Guid.NewGuid();
        var batataId = Guid.NewGuid();
        var refrigeranteId = Guid.NewGuid();

        var produtos = new List<Produto>
        {
            CriarProduto("X-Burger", 20m, 1, false, sanduicheId),
            CriarProduto("Batata", 10m, 2, true, batataId),
            CriarProduto("Refrigerante", 5m, 3, true, refrigeranteId)
        };

        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(produtos);

        var resultado = await _service.ValidarPedido([sanduicheId, batataId, refrigeranteId]);

        Assert.NotNull(resultado);
        Assert.Equal(35m, resultado!.Subtotal);
        Assert.Equal(20m, resultado.DescontoPorcentagem);
        Assert.Equal(28m, resultado.Total);
    }

    [Fact]
    public async Task InserirPedido_DeveCriarPedidoComProximoCodigoEItens()
    {
        var sanduicheId = Guid.NewGuid();
        var batataId = Guid.NewGuid();
        Pedido? pedidoInserido = null;
        List<PedidoProduto>? itensInseridos = null;

        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>
            {
                CriarProduto("X-Salada", 25m, 1, false, sanduicheId),
                CriarProduto("Batata", 10m, 2, true, batataId)
            });

        _pedidoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Pedido> { CriarPedido(codigo: 7) });

        _pedidoRepositoryMock
            .Setup(r => r.Inserir(It.IsAny<Pedido>(), It.IsAny<IEnumerable<PedidoProduto>>()))
            .ReturnsAsync((Pedido pedido, IEnumerable<PedidoProduto> itens) =>
            {
                pedidoInserido = pedido;
                itensInseridos = itens.ToList();
                return pedido;
            });

        var resultado = await _service.InserirPedido([sanduicheId, batataId]);

        Assert.NotNull(resultado);
        Assert.Equal(8, resultado!.Codigo);
        Assert.Equal(StatusPedidoEnum.CRIADO.ToString(), resultado.Status);
        Assert.NotNull(pedidoInserido);
        Assert.NotNull(itensInseridos);
        Assert.Equal(2, itensInseridos!.Count);
    }

    [Fact]
    public async Task InserirPedido_NaoDeveInserir_QuandoValidacaoFalha()
    {
        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>());

        var resultado = await _service.InserirPedido([Guid.NewGuid()]);

        Assert.Null(resultado);
        _pedidoRepositoryMock.Verify(r => r.Inserir(It.IsAny<Pedido>(), It.IsAny<IEnumerable<PedidoProduto>>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarPedido_DeveNotificarErro_QuandoPedidoNaoExiste()
    {
        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(It.IsAny<Guid>()))
            .ReturnsAsync((Pedido?)null);

        var resultado = await _service.AtualizarPedido(Guid.NewGuid(), [Guid.NewGuid()]);

        Assert.Null(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Pedido não encontrado.");
    }

    [Fact]
    public async Task AtualizarPedido_DeveNotificarErro_QuandoPedidoFinalizado()
    {
        var pedidoFinalizado = CriarPedido(codigo: 3, status: StatusPedidoEnum.FINALIZADO.ToString());

        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(It.IsAny<Guid>()))
            .ReturnsAsync(pedidoFinalizado);

        var resultado = await _service.AtualizarPedido(Guid.NewGuid(), [Guid.NewGuid()]);

        Assert.Null(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Não é possível editar um pedido finalizado.");
    }

    [Fact]
    public async Task AtualizarPedido_DeveAtualizarValoresEItens_QuandoPedidoValido()
    {
        var pedido = CriarPedido(codigo: 5);
        var sanduicheId = Guid.NewGuid();
        var refrigeranteId = Guid.NewGuid();
        Pedido? pedidoAtualizado = null;
        List<PedidoProduto>? itensAtualizados = null;

        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(pedido.Id))
            .ReturnsAsync(pedido);

        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>
            {
                CriarProduto("X-Burger", 20m, 1, false, sanduicheId),
                CriarProduto("Refrigerante", 6m, 3, true, refrigeranteId)
            });

        _pedidoRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Pedido>(), It.IsAny<IEnumerable<PedidoProduto>>()))
            .ReturnsAsync((Pedido pedidoRetornado, IEnumerable<PedidoProduto> itens) =>
            {
                pedidoAtualizado = pedidoRetornado;
                itensAtualizados = itens.ToList();
                return pedidoRetornado;
            });

        var resultado = await _service.AtualizarPedido(pedido.Id, [sanduicheId, refrigeranteId]);

        Assert.NotNull(resultado);
        Assert.Equal(26m, resultado!.Subtotal);
        Assert.Equal(15m, resultado.DescontoPorcentagem);
        Assert.Equal(22.1m, resultado.Total);
        Assert.NotNull(pedidoAtualizado);
        Assert.NotNull(itensAtualizados);
        Assert.Equal(2, itensAtualizados!.Count);
    }

    [Fact]
    public async Task FinalizarPedido_DeveNotificarErro_QuandoPedidoNaoExiste()
    {
        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(It.IsAny<Guid>()))
            .ReturnsAsync((Pedido?)null);

        var resultado = await _service.FinalizarPedido(Guid.NewGuid());

        Assert.Null(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Pedido não encontrado.");
    }

    [Fact]
    public async Task FinalizarPedido_DeveNotificarErro_QuandoPedidoJaFinalizado()
    {
        var pedidoFinalizado = CriarPedido(codigo: 2, status: StatusPedidoEnum.FINALIZADO.ToString());

        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(It.IsAny<Guid>()))
            .ReturnsAsync(pedidoFinalizado);

        var resultado = await _service.FinalizarPedido(Guid.NewGuid());

        Assert.Null(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "O pedido já está finalizado.");
    }

    [Fact]
    public async Task FinalizarPedido_DeveFinalizarEAtualizar_QuandoPedidoValido()
    {
        var pedido = CriarPedido(codigo: 2);

        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(pedido.Id))
            .ReturnsAsync(pedido);

        _pedidoRepositoryMock
            .Setup(r => r.Atualizar(It.IsAny<Pedido>()))
            .ReturnsAsync((Pedido pedidoAtualizado) => pedidoAtualizado);

        var resultado = await _service.FinalizarPedido(pedido.Id);

        Assert.NotNull(resultado);
        Assert.Equal(StatusPedidoEnum.FINALIZADO.ToString(), resultado!.Status);
        Assert.NotNull(resultado.DataFinalizacao);
    }

    [Fact]
    public async Task ExcluirPedido_DeveRetornarFalseEErro_QuandoPedidoNaoExiste()
    {
        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(It.IsAny<Guid>()))
            .ReturnsAsync((Pedido?)null);

        var resultado = await _service.ExcluirPedido(Guid.NewGuid());

        Assert.False(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Pedido não encontrado.");
    }

    [Fact]
    public async Task ExcluirPedido_DeveRetornarFalseEErro_QuandoRepositorioFalha()
    {
        var pedido = CriarPedido(codigo: 9);

        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(pedido.Id))
            .ReturnsAsync(pedido);

        _pedidoRepositoryMock
            .Setup(r => r.Excluir(pedido.Id))
            .ReturnsAsync(false);

        var resultado = await _service.ExcluirPedido(pedido.Id);

        Assert.False(resultado);
        Assert.Contains(_notificador.ObterNotificacoes(), n => n.Mensagem == "Erro ao excluir pedido.");
    }

    [Fact]
    public async Task ExcluirPedido_DeveRetornarTrue_QuandoExclusaoSucesso()
    {
        var pedido = CriarPedido(codigo: 11);

        _pedidoRepositoryMock
            .Setup(r => r.BuscarPorId(pedido.Id))
            .ReturnsAsync(pedido);

        _pedidoRepositoryMock
            .Setup(r => r.Excluir(pedido.Id))
            .ReturnsAsync(true);

        var resultado = await _service.ExcluirPedido(pedido.Id);

        Assert.True(resultado);
    }

    private static Produto CriarProduto(string nome, decimal preco, int codigo, bool acompanhamento, Guid id)
    {
        return new Produto(nome, preco, codigo, acompanhamento)
        {
            Id = id
        };
    }

    private static Pedido CriarPedido(int codigo, string? status = null)
    {
        return new Pedido(
            status: status ?? StatusPedidoEnum.CRIADO.ToString(),
            codigo: codigo,
            total: 0m,
            subtotal: 0m,
            descontoPorcentagem: 0m);
    }
}
