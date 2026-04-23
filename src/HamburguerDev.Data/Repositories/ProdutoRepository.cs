using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Models;
using HamburguerDev.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HamburguerDev.Data.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;

    public ProdutoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Produto>> Buscar()
    {
        return await _context.Produtos
            .AsNoTracking()
            .ToListAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
