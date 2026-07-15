using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Enums;
using ShoppingCart.Domain.Exceptions;

namespace ShoppingCart.Domain.Entities;

public sealed class Cart
{
    private readonly List<CartItem> _items = [];

    public Cart()
    {
        Id = Guid.NewGuid();
        Status = CartStatus.Open;

        RecalculateTotals();
    }

    public Guid Id { get; private set; }

    public CartStatus Status { get; private set; }

    public IReadOnlyCollection<CartItem> Items =>
        _items.AsReadOnly();

    public int? CouponId { get; private set; }

    public Coupon? AppliedCoupon { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal Discount { get; private set; }

    public decimal Total { get; private set; }

    public void AddProduct(
        Product product,
        int quantity)
    {
        EnsureOpen();

        ArgumentNullException.ThrowIfNull(product);

        if (quantity <= 0)
        {
            throw new DomainException(
                "A quantidade adicionada deve ser maior que zero.");
        }

        var existingItem = FindItem(product.Id);

        if (existingItem is null)
        {
            var newItem = new CartItem(product, quantity);

            _items.Add(newItem);
        }
        else
        {
            existingItem.AddQuantity(
                quantity,
                product.StockQuantity);
        }

        RecalculateTotals();
    }

    public void RemoveProduct(int productId)
    {
        EnsureOpen();

        var item = FindItem(productId);

        if (item is null)
        {
            throw new DomainException(
                $"O produto de identificador {productId} não existe no carrinho.");
        }

        _items.Remove(item);

        RecalculateTotals();
    }

    public void ChangeProductQuantity(
        int productId,
        int quantity)
    {
        EnsureOpen();

        var item = FindItem(productId);

        if (item is null)
        {
            throw new DomainException(
                $"O produto de identificador {productId} não existe no carrinho.");
        }

        item.SetQuantity(
            quantity,
            item.Product.StockQuantity);

        RecalculateTotals();
    }

    public void ApplyCoupon(Coupon coupon)
    {
        EnsureOpen();

        ArgumentNullException.ThrowIfNull(coupon);

        AppliedCoupon = coupon;
        CouponId = coupon.Id;

        RecalculateTotals();
    }

    public void RemoveCoupon()
    {
        EnsureOpen();

        AppliedCoupon = null;
        CouponId = null;

        RecalculateTotals();
    }

    public void Checkout()
    {
        EnsureOpen();

        Status = CartStatus.Finalized;
    }

    private CartItem? FindItem(int productId)
    {
        return _items.FirstOrDefault(
            item => item.ProductId == productId);
    }

    private void RecalculateTotals()
    {
        Subtotal = Money.Round(
            _items.Sum(item => item.TotalPrice));

        Discount = AppliedCoupon is null
            ? 0m
            : AppliedCoupon.CalculateDiscount(Subtotal);

        Total = Money.Round(Subtotal - Discount);
    }

    private void EnsureOpen()
    {
        if (Status == CartStatus.Finalized)
        {
            throw new DomainException(
                "O carrinho está finalizado e não pode mais ser alterado.");
        }
    }
}