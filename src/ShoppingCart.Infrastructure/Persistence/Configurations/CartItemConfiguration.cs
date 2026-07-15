using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure.Persistence.Configurations;

internal sealed class CartItemConfiguration
    : IEntityTypeConfiguration<CartItem>
{
    public void Configure(
        EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(
            "CarrinhoItem",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_CarrinhoItem_Quantidade_Positiva",
                    "[Quantidade] > 0");

                tableBuilder.HasCheckConstraint(
                    "CK_CarrinhoItem_PrecoUnitario_NaoNegativo",
                    "[PrecoUnitario] >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_CarrinhoItem_PrecoTotal_NaoNegativo",
                    "[PrecoTotal] >= 0");
            });

        builder.HasKey(item => item.Id)
            .HasName("PK_CarrinhoItem");

        builder.Property(item => item.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property<Guid>("CartId")
            .HasColumnName("CarrinhoID")
            .IsRequired();

        builder.Property(item => item.ProductId)
            .HasColumnName("ProdutoID")
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasColumnName("Quantidade")
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasColumnName("PrecoUnitario")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.TotalPrice)
            .HasColumnName("PrecoTotal")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_CarrinhoItem_Produto");

        builder.HasIndex(
                "CartId",
                nameof(CartItem.ProductId))
            .IsUnique()
            .HasDatabaseName(
                "UX_CarrinhoItem_Carrinho_Produto");
    }
}