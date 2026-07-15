using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure.Persistence.Configurations;

internal sealed class CartConfiguration
    : IEntityTypeConfiguration<Cart>
{
    public void Configure(
        EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable(
            "Carrinho",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Carrinho_Subtotal_NaoNegativo",
                    "[Subtotal] >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_Carrinho_Desconto_NaoNegativo",
                    "[Desconto] >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_Carrinho_Total_Valido",
                    "[Total] >= 0 AND [Total] <= [Subtotal]");
            });

        builder.HasKey(cart => cart.Id)
            .HasName("PK_Carrinho");

        builder.Property(cart => cart.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(cart => cart.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(cart => cart.CouponId)
            .HasColumnName("CupomID")
            .IsRequired(false);

        builder.Property(cart => cart.Subtotal)
            .HasColumnName("Subtotal")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cart => cart.Discount)
            .HasColumnName("Desconto")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cart => cart.Total)
            .HasColumnName("Total")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(cart => cart.AppliedCoupon)
            .WithMany()
            .HasForeignKey(cart => cart.CouponId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Carrinho_Cupom");

        builder.HasMany(cart => cart.Items)
            .WithOne()
            .HasForeignKey("CartId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CarrinhoItem_Carrinho");

        builder.Navigation(cart => cart.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}