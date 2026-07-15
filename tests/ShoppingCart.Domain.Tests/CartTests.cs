using ShoppingCart.Domain.Entities;
using ShoppingCart.Domain.Enums;
using ShoppingCart.Domain.Exceptions;

namespace Domain.Tests;

public sealed class CartTests
{
    [Fact]
    public void Constructor_ShouldCreateOpenCart()
    {
        var cart = new Cart();

        Assert.NotEqual(Guid.Empty, cart.Id);
        Assert.Equal(CartStatus.Open, cart.Status);
        Assert.Empty(cart.Items);
        Assert.Equal(0m, cart.Subtotal);
        Assert.Equal(0m, cart.Discount);
        Assert.Equal(0m, cart.Total);
    }

    [Fact]
    public void AddProduct_ShouldAddNewItemAndCalculateTotals()
    {
        var cart = new Cart();
        var product = CreateProduct(
            id: 1,
            stock: 10,
            price: 25.50m);

        cart.AddProduct(product, 2);

        var item = Assert.Single(cart.Items);

        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(25.50m, item.UnitPrice);
        Assert.Equal(51.00m, item.TotalPrice);

        Assert.Equal(51.00m, cart.Subtotal);
        Assert.Equal(0m, cart.Discount);
        Assert.Equal(51.00m, cart.Total);
    }

    [Fact]
    public void AddProduct_WhenProductAlreadyExists_ShouldSumQuantities()
    {
        var cart = new Cart();
        var product = CreateProduct(
            id: 1,
            stock: 10,
            price: 10m);

        cart.AddProduct(product, 2);
        cart.AddProduct(product, 3);

        var item = Assert.Single(cart.Items);

        Assert.Equal(5, item.Quantity);
        Assert.Equal(50m, item.TotalPrice);
        Assert.Equal(50m, cart.Subtotal);
        Assert.Equal(50m, cart.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddProduct_WithInvalidQuantity_ShouldThrow(
        int quantity)
    {
        var cart = new Cart();
        var product = CreateProduct();

        var exception = Assert.Throws<DomainException>(
            () => cart.AddProduct(product, quantity));

        Assert.Contains(
            "maior que zero",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddProduct_WhenQuantityExceedsStock_ShouldThrow()
    {
        var cart = new Cart();

        var product = CreateProduct(
            stock: 2);

        var exception = Assert.Throws<DomainException>(
            () => cart.AddProduct(product, 3));

        Assert.Contains(
            "estoque",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddProduct_WhenAccumulatedQuantityExceedsStock_ShouldThrow()
    {
        var cart = new Cart();

        var product = CreateProduct(
            stock: 5);

        cart.AddProduct(product, 3);

        var exception = Assert.Throws<DomainException>(
            () => cart.AddProduct(product, 3));

        Assert.Contains(
            "estoque",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);

        Assert.Equal(
            3,
            Assert.Single(cart.Items).Quantity);
    }

    [Fact]
    public void ChangeProductQuantity_ShouldReplaceQuantity()
    {
        var cart = new Cart();

        var product = CreateProduct(
            stock: 10,
            price: 12.50m);

        cart.AddProduct(product, 2);
        cart.ChangeProductQuantity(product.Id, 5);

        var item = Assert.Single(cart.Items);

        Assert.Equal(5, item.Quantity);
        Assert.Equal(62.50m, item.TotalPrice);
        Assert.Equal(62.50m, cart.Subtotal);
        Assert.Equal(62.50m, cart.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void ChangeProductQuantity_WithInvalidQuantity_ShouldThrow(
        int quantity)
    {
        var cart = new Cart();
        var product = CreateProduct();

        cart.AddProduct(product, 1);

        Assert.Throws<DomainException>(
            () => cart.ChangeProductQuantity(
                product.Id,
                quantity));
    }

    [Fact]
    public void ChangeProductQuantity_WhenProductDoesNotExist_ShouldThrow()
    {
        var cart = new Cart();

        var exception = Assert.Throws<DomainException>(
            () => cart.ChangeProductQuantity(999, 2));

        Assert.Contains(
            "não existe",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RemoveProduct_ShouldRemoveItemAndRecalculateTotals()
    {
        var cart = new Cart();

        var firstProduct = CreateProduct(
            id: 1,
            price: 10m);

        var secondProduct = CreateProduct(
            id: 2,
            price: 20m);

        cart.AddProduct(firstProduct, 2);
        cart.AddProduct(secondProduct, 1);

        cart.RemoveProduct(firstProduct.Id);

        var remainingItem = Assert.Single(cart.Items);

        Assert.Equal(secondProduct.Id, remainingItem.ProductId);
        Assert.Equal(20m, cart.Subtotal);
        Assert.Equal(20m, cart.Total);
    }

    [Fact]
    public void RemoveProduct_WhenProductDoesNotExist_ShouldThrow()
    {
        var cart = new Cart();

        var exception = Assert.Throws<DomainException>(
            () => cart.RemoveProduct(999));

        Assert.Contains(
            "não existe",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ApplyCoupon_ShouldCalculateDiscountAndTotal()
    {
        var cart = new Cart();

        var product = CreateProduct(
            price: 100m);

        var coupon = new Coupon(
            id: 1,
            code: "10off",
            discountPercentage: 10m);

        cart.AddProduct(product, 2);
        cart.ApplyCoupon(coupon);

        Assert.Equal("10OFF", cart.AppliedCoupon?.Code);
        Assert.Equal(200m, cart.Subtotal);
        Assert.Equal(20m, cart.Discount);
        Assert.Equal(180m, cart.Total);
    }

    [Fact]
    public void ApplyCoupon_ShouldReplacePreviousCoupon()
    {
        var cart = new Cart();

        var product = CreateProduct(
            price: 100m);

        var tenOff = new Coupon(
            id: 1,
            code: "10OFF",
            discountPercentage: 10m);

        var fifteenOff = new Coupon(
            id: 2,
            code: "15OFF",
            discountPercentage: 15m);

        cart.AddProduct(product, 2);

        cart.ApplyCoupon(tenOff);
        cart.ApplyCoupon(fifteenOff);

        Assert.Equal(2, cart.CouponId);
        Assert.Equal("15OFF", cart.AppliedCoupon?.Code);
        Assert.Equal(200m, cart.Subtotal);
        Assert.Equal(30m, cart.Discount);
        Assert.Equal(170m, cart.Total);
    }

    [Fact]
    public void RemoveCoupon_ShouldClearDiscountAndRecalculateTotal()
    {
        var cart = new Cart();

        var product = CreateProduct(
            price: 100m);

        var coupon = new Coupon(
            id: 1,
            code: "10OFF",
            discountPercentage: 10m);

        cart.AddProduct(product, 1);
        cart.ApplyCoupon(coupon);

        cart.RemoveCoupon();

        Assert.Null(cart.CouponId);
        Assert.Null(cart.AppliedCoupon);
        Assert.Equal(100m, cart.Subtotal);
        Assert.Equal(0m, cart.Discount);
        Assert.Equal(100m, cart.Total);
    }

    [Fact]
    public void Coupon_ShouldUseAwayFromZeroRounding()
    {
        var coupon = new Coupon(
            id: 1,
            code: "10OFF",
            discountPercentage: 10m);

        var discount = coupon.CalculateDiscount(10.05m);

        Assert.Equal(1.01m, discount);
    }

    [Fact]
    public void Checkout_ShouldFinalizeCart()
    {
        var cart = new Cart();

        cart.Checkout();

        Assert.Equal(CartStatus.Finalized, cart.Status);
    }

    [Fact]
    public void FinalizedCart_ShouldNotAllowAddingProducts()
    {
        var cart = new Cart();
        var product = CreateProduct();

        cart.Checkout();

        var exception = Assert.Throws<DomainException>(
            () => cart.AddProduct(product, 1));

        Assert.Contains(
            "finalizado",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FinalizedCart_ShouldNotAllowRemovingProducts()
    {
        var cart = new Cart();
        var product = CreateProduct();

        cart.AddProduct(product, 1);
        cart.Checkout();

        Assert.Throws<DomainException>(
            () => cart.RemoveProduct(product.Id));
    }

    [Fact]
    public void FinalizedCart_ShouldNotAllowChangingQuantity()
    {
        var cart = new Cart();
        var product = CreateProduct();

        cart.AddProduct(product, 1);
        cart.Checkout();

        Assert.Throws<DomainException>(
            () => cart.ChangeProductQuantity(
                product.Id,
                2));
    }

    [Fact]
    public void FinalizedCart_ShouldNotAllowChangingCoupon()
    {
        var cart = new Cart();

        var coupon = new Coupon(
            id: 1,
            code: "10OFF",
            discountPercentage: 10m);

        cart.Checkout();

        Assert.Throws<DomainException>(
            () => cart.ApplyCoupon(coupon));
    }

    [Fact]
    public void Checkout_WhenAlreadyFinalized_ShouldThrow()
    {
        var cart = new Cart();

        cart.Checkout();

        Assert.Throws<DomainException>(
            cart.Checkout);
    }

    private static Product CreateProduct(
        int id = 1,
        int stock = 10,
        decimal price = 10m)
    {
        return new Product(
            id,
            $"Produto {id}",
            stock,
            price);
    }
}