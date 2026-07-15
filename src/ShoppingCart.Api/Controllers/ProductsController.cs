using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Api.Contracts.Products;
using ShoppingCart.Api.Mappings;
using ShoppingCart.Application.UseCases.Products;

namespace ShoppingCart.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ListProductsUseCase _listProductsUseCase;

    public ProductsController(
        ListProductsUseCase listProductsUseCase)
    {
        _listProductsUseCase = listProductsUseCase;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<ProductResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<
        IReadOnlyCollection<ProductResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var output = await _listProductsUseCase.ExecuteAsync(
            cancellationToken);

        var response = output
            .Select(product => product.ToResponse())
            .ToArray();

        return Ok(response);
    }
}