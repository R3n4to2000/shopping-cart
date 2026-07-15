using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Domain.Entities;
using ShoppingCart.Infrastructure.Persistence.Seed;

namespace ShoppingCart.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration
    : IEntityTypeConfiguration<Product>
{
    public void Configure(
        EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(
            "Produto",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Produto_QuantidadeEstoque_NaoNegativa",
                    "[QuantidadeEstoque] >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_Produto_PrecoLiquido_NaoNegativo",
                    "[PrecoLiquido] >= 0");
            });

        builder.HasKey(product => product.Id)
            .HasName("PK_Produto");

        builder.Property(product => product.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(product => product.Description)
            .HasColumnName("DescricaoProduto")
            .HasMaxLength(200)
            .IsUnicode()
            .IsRequired();

        builder.Property(product => product.StockQuantity)
            .HasColumnName("QuantidadeEstoque")
            .IsRequired();

        builder.Property(product => product.NetPrice)
            .HasColumnName("PrecoLiquido")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasData(CatalogSeed.Products);
    }
}