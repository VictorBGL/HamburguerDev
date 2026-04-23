using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Models;

namespace HamburguerDev.Business.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<Produto>> Buscar(int? codigo = null, string? nome = null)
    {
        var produtos = await _produtoRepository.Buscar();

        if (codigo.HasValue)
        {
            produtos = produtos.Where(p => p.Codigo == codigo.Value);
        }

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var nomeFiltro = nome.Trim();
            produtos = produtos.Where(p => p.Nome.Contains(nomeFiltro, StringComparison.OrdinalIgnoreCase));
        }

        return produtos.OrderBy(p => p.Codigo);
    }

    public void Dispose()
    {
        _produtoRepository.Dispose();
    }
}
