using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Domain.Entities;
using ShoppingCart.Infrastructure.Persistence.Seed;

namespace ShoppingCart.Infrastructure.Persistence.Configurations;

internal sealed class CouponConfiguration
    : IEntityTypeConfiguration<Coupon>
{
    public void Configure(
        EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable(
            "Cupom",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Cupom_PercentualDesconto_Valido",
                    "[PercentualDesconto] > 0 " +
                    "AND [PercentualDesconto] <= 100");
            });

        builder.HasKey(coupon => coupon.Id)
            .HasName("PK_Cupom");

        builder.Property(coupon => coupon.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(coupon => coupon.Code)
            .HasColumnName("CodigoCupom")
            .HasMaxLength(30)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(coupon => coupon.DiscountPercentage)
            .HasColumnName("PercentualDesconto")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.HasIndex(coupon => coupon.Code)
            .IsUnique()
            .HasDatabaseName("UX_Cupom_CodigoCupom");

        builder.HasData(CatalogSeed.Coupons);
    }
}