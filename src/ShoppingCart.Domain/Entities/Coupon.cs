using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Exceptions;

namespace ShoppingCart.Domain.Entities;

public sealed class Coupon
{
    private Coupon()
    {
        Code = string.Empty;
    }

    public Coupon(
        int id,
        string code,
        decimal discountPercentage)
    {
        if (id <= 0)
        {
            throw new DomainException(
                "O identificador do cupom deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException(
                "O código do cupom é obrigatório.");
        }

        if (discountPercentage <= 0 || discountPercentage > 100)
        {
            throw new DomainException(
                "O percentual de desconto deve ser maior que zero e menor ou igual a 100.");
        }

        Id = id;
        Code = code.Trim().ToUpperInvariant();
        DiscountPercentage = discountPercentage;
    }

    public int Id { get; private set; }

    public string Code { get; private set; }

    public decimal DiscountPercentage { get; private set; }

    public decimal CalculateDiscount(decimal subtotal)
    {
        if (subtotal <= 0)
        {
            return 0m;
        }

        var discount = subtotal * DiscountPercentage / 100m;

        return Money.Round(discount);
    }
}