using Application.Tests.Fakes;
using ShoppingCart.Application.UseCases.Products;
using ShoppingCart.Domain.Entities;
using Xunit;

namespace Application.Tests.UseCases;

public sealed class ProductUseCasesTests
{
    [Fact]
    public async Task ListProducts_ShouldReturnProductsOrderedById()
    {
        var secondProduct = new Product(
            id: 2,
            description: "Produto 2",
            stockQuantity: 5,
            netPrice: 20m);

        var firstProduct = new Product(
            id: 1,
            description: "Produto 1",
            stockQuantity: 10,
            netPrice: 10m);

        var repository = new FakeProductRepository(
            secondProduct,
            firstProduct);

        var useCase = new ListProductsUseCase(
            repository);

        var result = await useCase.ExecuteAsync();

        Assert.Equal(2, result.Count);

        Assert.Collection(
            result,
            product =>
            {
                Assert.Equal(1, product.Id);
                Assert.Equal("Produto 1", product.Description);
                Assert.Equal(10, product.AvailableStock);
                Assert.Equal(10m, product.NetPrice);
            },
            product =>
            {
                Assert.Equal(2, product.Id);
                Assert.Equal("Produto 2", product.Description);
                Assert.Equal(5, product.AvailableStock);
                Assert.Equal(20m, product.NetPrice);
            });
    }
}