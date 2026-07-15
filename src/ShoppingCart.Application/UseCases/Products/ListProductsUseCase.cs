using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Application.Common.Mappings;
using ShoppingCart.Application.Models.Outputs;

namespace ShoppingCart.Application.UseCases.Products;

public sealed class ListProductsUseCase
{
    private readonly IProductRepository _productRepository;

    public ListProductsUseCase(
        IProductRepository productRepository)
    {
        _productRepository = productRepository
            ?? throw new ArgumentNullException(
                nameof(productRepository));
    }

    public async Task<IReadOnlyCollection<ProductOutput>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(
            cancellationToken);

        return products
            .OrderBy(product => product.Id)
            .Select(product => product.ToOutput())
            .ToArray();
    }
}