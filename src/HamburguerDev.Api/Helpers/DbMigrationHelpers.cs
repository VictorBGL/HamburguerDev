using HamburguerDev.Business.Models;
using HamburguerDev.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HamburguerDev.Api.Helpers;

public static class DbMigrationHelperExtension
{
    public static void UseDbMigrationHelper(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        SeedProdutos(context);
    }

    private static void SeedProdutos(ApplicationDbContext context)
    {
        if (context.Produtos.Any())
        {
            return;
        }

        var produtos = new List<Produto>
        {
            new("X Burger", 5.00m, codigo: 1, acompanhamento: false),
            new("X Egg", 4.50m, codigo: 2, acompanhamento: false),
            new("X Bacon", 7.00m, codigo: 3, acompanhamento: false),
            new("Batata Frita", 2.00m, codigo: 4, acompanhamento: true),
            new("Refrigerante", 2.50m, codigo: 5, acompanhamento: true),
        };

        context.Produtos.AddRange(produtos);
        context.SaveChanges();
    }
}
