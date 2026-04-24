using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Models;
using HamburguerDev.Business.Services;
using Moq;

namespace HamburguerDev.Business.Tests;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly ProdutoService _service;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _service = new ProdutoService(_produtoRepositoryMock.Object);
    }

    [Fact]
    public async Task Buscar_DeveOrdenarPorCodigo_QuandoSemFiltros()
    {
        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>
            {
                CriarProduto("Refrigerante", 7m, 3, true),
                CriarProduto("X-Burger", 20m, 1, false),
                CriarProduto("Batata", 10m, 2, true)
            });

        var resultado = (await _service.Buscar()).ToList();

        Assert.Equal(3, resultado.Count);
        Assert.Equal(new[] { 1, 2, 3 }, resultado.Select(p => p.Codigo));
    }

    [Fact]
    public async Task Buscar_DeveFiltrarPorCodigo_QuandoCodigoInformado()
    {
        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>
            {
                CriarProduto("X-Burger", 20m, 1, false),
                CriarProduto("Batata", 10m, 2, true)
            });

        var resultado = await _service.Buscar(codigo: 2);

        Assert.Single(resultado);
        Assert.Equal("Batata", resultado.First().Nome);
    }

    [Fact]
    public async Task Buscar_DeveFiltrarPorNome_IgnoreCaseETrim_QuandoNomeInformado()
    {
        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>
            {
                CriarProduto("Batata Frita", 10m, 2, true),
                CriarProduto("X-Burger", 20m, 1, false),
                CriarProduto("Refrigerante", 7m, 3, true)
            });

        var resultado = await _service.Buscar(nome: "  bAtAtA ");

        Assert.Single(resultado);
        Assert.Equal("Batata Frita", resultado.First().Nome);
    }

    [Fact]
    public async Task Buscar_DeveAplicarFiltrosCombinados_QuandoCodigoENomeInformados()
    {
        _produtoRepositoryMock
            .Setup(r => r.Buscar())
            .ReturnsAsync(new List<Produto>
            {
                CriarProduto("Batata Frita", 10m, 2, true),
                CriarProduto("Batata Rustica", 12m, 5, true),
                CriarProduto("X-Burger", 20m, 1, false)
            });

        var resultado = await _service.Buscar(codigo: 5, nome: "batata");

        Assert.Single(resultado);
        Assert.Equal(5, resultado.First().Codigo);
        Assert.Equal("Batata Rustica", resultado.First().Nome);
    }

    private static Produto CriarProduto(string nome, decimal preco, int codigo, bool acompanhamento)
    {
        return new Produto(nome, preco, codigo, acompanhamento);
    }
}
