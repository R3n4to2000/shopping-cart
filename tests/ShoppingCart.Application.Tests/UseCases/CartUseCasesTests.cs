using Application.Tests.Fakes;
using ShoppingCart.Application.Common.Exceptions;
using ShoppingCart.Application.Models.Inputs;
using ShoppingCart.Application.UseCases.Carts;
using ShoppingCart.Domain.Entities;
using Xunit;

namespace Application.Tests.UseCases;

public sealed class CartUseCasesTests
{
    [Fact]
    public async Task CreateCart_ShouldPersistOpenCart()
    {
        var cartRepository = new FakeCartRepository();
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new CreateCartUseCase(
            cartRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync();

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Open", result.Status);
        Assert.Empty(result.Items);
        Assert.Equal(0m, result.Subtotal);
        Assert.Equal(0m, result.Discount);
        Assert.Equal(0m, result.Total);

        Assert.Single(cartRepository.Carts);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task GetCart_ShouldReturnCompleteCartOutput()
    {
        var product = CreateProduct(
            id: 1,
            stock: 8,
            price: 25m);

        var coupon = new Coupon(
            id: 1,
            code: "10OFF",
            discountPercentage: 10m);

        var cart = new Cart();

        cart.AddProduct(product, 2);
        cart.ApplyCoupon(coupon);

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var useCase = new GetCartUseCase(
            cartRepository);

        var result = await useCase.ExecuteAsync(
            new GetCartInput(cart.Id));

        var item = Assert.Single(result.Items);

        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal("Produto 1", item.ProductDescription);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(8, item.AvailableStock);
        Assert.Equal(25m, item.NetUnitPrice);
        Assert.Equal(50m, item.TotalPrice);

        Assert.NotNull(result.AppliedCoupon);
        Assert.Equal("10OFF", result.AppliedCoupon.Code);

        Assert.Equal(50m, result.Subtotal);
        Assert.Equal(5m, result.Discount);
        Assert.Equal(45m, result.Total);
    }

    [Fact]
    public async Task GetCart_WhenCartDoesNotExist_ShouldThrow()
    {
        var repository = new FakeCartRepository();

        var useCase = new GetCartUseCase(repository);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(
                new GetCartInput(Guid.NewGuid())));
    }

    [Fact]
    public async Task AddProduct_ShouldAddProductAndSaveChanges()
    {
        var product = CreateProduct(
            id: 1,
            stock: 10,
            price: 15m);

        var cart = new Cart();

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var productRepository =
            new FakeProductRepository(product);

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new AddProductToCartUseCase(
            cartRepository,
            productRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new AddProductToCartInput(
                CartId: cart.Id,
                ProductId: product.Id,
                Quantity: 2));

        var item = Assert.Single(result.Items);

        Assert.Equal(2, item.Quantity);
        Assert.Equal(30m, item.TotalPrice);
        Assert.Equal(30m, result.Subtotal);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task AddProduct_WhenProductDoesNotExist_ShouldThrow()
    {
        var cart = new Cart();

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var productRepository =
            new FakeProductRepository();

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new AddProductToCartUseCase(
            cartRepository,
            productRepository,
            unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(
                new AddProductToCartInput(
                    CartId: cart.Id,
                    ProductId: 999,
                    Quantity: 1)));

        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task ChangeQuantity_ShouldReplaceExactQuantity()
    {
        var product = CreateProduct(
            stock: 10,
            price: 12m);

        var cart = new Cart();
        cart.AddProduct(product, 2);

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new ChangeCartItemQuantityUseCase(
            cartRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new ChangeCartItemQuantityInput(
                CartId: cart.Id,
                ProductId: product.Id,
                Quantity: 5));

        var item = Assert.Single(result.Items);

        Assert.Equal(5, item.Quantity);
        Assert.Equal(60m, item.TotalPrice);
        Assert.Equal(60m, result.Total);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task RemoveProduct_ShouldRemoveAndSaveChanges()
    {
        var firstProduct = CreateProduct(
            id: 1,
            price: 10m);

        var secondProduct = CreateProduct(
            id: 2,
            price: 20m);

        var cart = new Cart();

        cart.AddProduct(firstProduct, 1);
        cart.AddProduct(secondProduct, 1);

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new RemoveProductFromCartUseCase(
            cartRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new RemoveProductFromCartInput(
                CartId: cart.Id,
                ProductId: firstProduct.Id));

        var remainingItem = Assert.Single(result.Items);

        Assert.Equal(secondProduct.Id, remainingItem.ProductId);
        Assert.Equal(20m, result.Subtotal);
        Assert.Equal(20m, result.Total);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task ApplyCoupon_ShouldNormalizeAndApplyCoupon()
    {
        var product = CreateProduct(
            price: 100m);

        var coupon = new Coupon(
            id: 2,
            code: "15OFF",
            discountPercentage: 15m);

        var cart = new Cart();
        cart.AddProduct(product, 2);

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var couponRepository =
            new FakeCouponRepository(coupon);

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new ApplyCouponUseCase(
            cartRepository,
            couponRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new ApplyCouponInput(
                CartId: cart.Id,
                Code: " 15off "));

        Assert.NotNull(result.AppliedCoupon);
        Assert.Equal("15OFF", result.AppliedCoupon.Code);
        Assert.Equal(200m, result.Subtotal);
        Assert.Equal(30m, result.Discount);
        Assert.Equal(170m, result.Total);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task ApplyCoupon_WithEmptyCode_ShouldThrowValidation()
    {
        var cart = new Cart();

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var useCase = new ApplyCouponUseCase(
            cartRepository,
            new FakeCouponRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ValidationException>(
            () => useCase.ExecuteAsync(
                new ApplyCouponInput(
                    CartId: cart.Id,
                    Code: " ")));
    }

    [Fact]
    public async Task ApplyCoupon_WhenCouponDoesNotExist_ShouldThrow()
    {
        var cart = new Cart();

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new ApplyCouponUseCase(
            cartRepository,
            new FakeCouponRepository(),
            unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(
            () => useCase.ExecuteAsync(
                new ApplyCouponInput(
                    CartId: cart.Id,
                    Code: "INVALIDO")));

        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task RemoveCoupon_ShouldClearDiscount()
    {
        var product = CreateProduct(
            price: 100m);

        var coupon = new Coupon(
            id: 1,
            code: "10OFF",
            discountPercentage: 10m);

        var cart = new Cart();

        cart.AddProduct(product, 1);
        cart.ApplyCoupon(coupon);

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new RemoveCouponUseCase(
            cartRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new RemoveCouponInput(cart.Id));

        Assert.Null(result.AppliedCoupon);
        Assert.Equal(100m, result.Subtotal);
        Assert.Equal(0m, result.Discount);
        Assert.Equal(100m, result.Total);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Checkout_ShouldFinalizeCart()
    {
        var cart = new Cart();

        var cartRepository = new FakeCartRepository();
        cartRepository.Seed(cart);

        var unitOfWork = new FakeUnitOfWork();

        var useCase = new CheckoutCartUseCase(
            cartRepository,
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new CheckoutCartInput(cart.Id));

        Assert.Equal("Finalized", result.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    private static Product CreateProduct(
        int id = 1,
        int stock = 10,
        decimal price = 10m)
    {
        return new Product(
            id: id,
            description: $"Produto {id}",
            stockQuantity: stock,
            netPrice: price);
    }
}