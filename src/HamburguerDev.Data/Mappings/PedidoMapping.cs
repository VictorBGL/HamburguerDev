using HamburguerDev.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HamburguerDev.Data.Mappings;

public class PedidoMapping : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status)
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(p => p.Codigo)
            .IsRequired();

        builder.Property(p => p.Total)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Subtotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.DescontoPorcentagem)
            .HasColumnType("decimal(5,2)");

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        builder.Property(p => p.DataFinalizacao);

        builder.HasMany(p => p.PedidoProdutos)
            .WithOne(pp => pp.Pedido)
            .HasForeignKey(pp => pp.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
