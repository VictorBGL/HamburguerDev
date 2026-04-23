using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Models;
using HamburguerDev.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HamburguerDev.Data.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly ApplicationDbContext _context;

    public PedidoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pedido>> Buscar()
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.PedidoProdutos!)
            .ThenInclude(pp => pp.Produto)
            .ToListAsync();
    }

    public async Task<Pedido?> BuscarPorId(Guid id)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.PedidoProdutos!)
            .ThenInclude(pp => pp.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Pedido?> Inserir(Pedido pedido, IEnumerable<PedidoProduto> pedidoProdutos)
    {
        await _context.Pedidos.AddAsync(pedido);
        await _context.PedidosProdutos.AddRangeAsync(pedidoProdutos);
        await _context.SaveChangesAsync();

        return await BuscarPorId(pedido.Id);
    }

    public async Task<Pedido?> Atualizar(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();

        return await BuscarPorId(pedido.Id);
    }

    public async Task<bool> Excluir(Guid id)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido is null)
        {
            return false;
        }

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();
        return true;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
