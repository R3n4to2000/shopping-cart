using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Exceptions;

namespace ShoppingCart.Domain.Entities;

public sealed class Product
{
    private Product()
    {
        Description = string.Empty;
    }

    public Product(
        int id,
        string description,
        int stockQuantity,
        decimal netPrice)
    {
        if (id <= 0)
        {
            throw new DomainException(
                "O identificador do produto deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(
                "A descrição do produto é obrigatória.");
        }

        if (stockQuantity < 0)
        {
            throw new DomainException(
                "A quantidade em estoque não pode ser negativa.");
        }

        if (netPrice < 0)
        {
            throw new DomainException(
                "O preço líquido do produto não pode ser negativo.");
        }

        Id = id;
        Description = description.Trim();
        StockQuantity = stockQuantity;
        NetPrice = Money.Round(netPrice);
    }

    public int Id { get; private set; }

    public string Description { get; private set; }

    public int StockQuantity { get; private set; }

    public decimal NetPrice { get; private set; }
}