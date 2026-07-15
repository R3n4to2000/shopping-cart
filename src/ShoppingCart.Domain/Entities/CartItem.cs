using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Exceptions;

namespace ShoppingCart.Domain.Entities;

public sealed class CartItem
{
    private CartItem()
    {
        Product = null!;
    }

    internal CartItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);

        ValidateQuantity(quantity, product.StockQuantity);

        Id = Guid.NewGuid();
        Product = product;
        ProductId = product.Id;
        UnitPrice = Money.Round(product.NetPrice);
        Quantity = quantity;

        RecalculateTotal();
    }

    public Guid Id { get; private set; }

    public int ProductId { get; private set; }

    public Product Product { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice { get; private set; }

    internal void AddQuantity(
        int quantityToAdd,
        int availableStock)
    {
        if (quantityToAdd <= 0)
        {
            throw new DomainException(
                "A quantidade adicionada deve ser maior que zero.");
        }

        var newQuantity = Quantity + quantityToAdd;

        SetQuantity(newQuantity, availableStock);
    }

    internal void SetQuantity(
        int quantity,
        int availableStock)
    {
        ValidateQuantity(quantity, availableStock);

        Quantity = quantity;

        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalPrice = Money.Round(UnitPrice * Quantity);
    }

    private static void ValidateQuantity(
        int quantity,
        int availableStock)
    {
        if (quantity <= 0)
        {
            throw new DomainException(
                "A quantidade do produto deve ser maior que zero.");
        }

        if (quantity > availableStock)
        {
            throw new DomainException(
                $"A quantidade solicitada ({quantity}) ultrapassa o estoque disponível ({availableStock}).");
        }
    }
}