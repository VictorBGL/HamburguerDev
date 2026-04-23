using HamburguerDev.Business.Models;

namespace HamburguerDev.Business.Interfaces;

public interface IProdutoRepository : IDisposable
{
    Task<IEnumerable<Produto>> Buscar();
}
