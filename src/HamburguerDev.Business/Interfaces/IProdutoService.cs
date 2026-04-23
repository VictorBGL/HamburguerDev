using HamburguerDev.Business.Models;

namespace HamburguerDev.Business.Interfaces;

public interface IProdutoService : IDisposable
{
    Task<IEnumerable<Produto>> Buscar(int? codigo = null, string? nome = null);
}
