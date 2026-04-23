using HamburguerDev.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HamburguerDev.Data.Mappings;

public class PedidoProdutoMapping : IEntityTypeConfiguration<PedidoProduto>
{
    public void Configure(EntityTypeBuilder<PedidoProduto> builder)
    {
        builder.ToTable("PedidosProdutos");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.PedidoId)
            .IsRequired();

        builder.Property(pp => pp.ProdutoId)
            .IsRequired();

        builder.HasOne(pp => pp.Pedido)
            .WithMany(p => p.PedidoProdutos)
            .HasForeignKey(pp => pp.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Produto)
            .WithMany(p => p.PedidosProduto)
            .HasForeignKey(pp => pp.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
