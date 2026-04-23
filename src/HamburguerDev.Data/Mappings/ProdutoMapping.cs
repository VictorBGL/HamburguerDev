using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using HamburguerDev.Business.Models;

namespace HamburguerDev.Data.Mappings
{
    public class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Nome).HasColumnType("varchar(100)").IsRequired();
            builder.Property(c => c.Preco).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(c => c.Acompanhamento).IsRequired().HasDefaultValue(false);
        }
    }
}
