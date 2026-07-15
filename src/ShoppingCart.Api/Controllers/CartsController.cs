using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Api.Contracts.Carts;
using ShoppingCart.Api.Mappings;
using ShoppingCart.Application.Models.Inputs;
using ShoppingCart.Application.UseCases.Carts;

namespace ShoppingCart.Api.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartsController : ControllerBase
{
    private readonly CreateCartUseCase _createCartUseCase;
    private readonly GetCartUseCase _getCartUseCase;
    private readonly AddProductToCartUseCase
        _addProductToCartUseCase;
    private readonly ChangeCartItemQuantityUseCase
        _changeCartItemQuantityUseCase;
    private readonly RemoveProductFromCartUseCase
        _removeProductFromCartUseCase;
    private readonly ApplyCouponUseCase _applyCouponUseCase;
    private readonly RemoveCouponUseCase _removeCouponUseCase;
    private readonly CheckoutCartUseCase _checkoutCartUseCase;

    public CartsController(
        CreateCartUseCase createCartUseCase,
        GetCartUseCase getCartUseCase,
        AddProductToCartUseCase addProductToCartUseCase,
        ChangeCartItemQuantityUseCase
            changeCartItemQuantityUseCase,
        RemoveProductFromCartUseCase
            removeProductFromCartUseCase,
        ApplyCouponUseCase applyCouponUseCase,
        RemoveCouponUseCase removeCouponUseCase,
        CheckoutCartUseCase checkoutCartUseCase)
    {
        _createCartUseCase = createCartUseCase;
        _getCartUseCase = getCartUseCase;
        _addProductToCartUseCase = addProductToCartUseCase;
        _changeCartItemQuantityUseCase =
            changeCartItemQuantityUseCase;
        _removeProductFromCartUseCase =
            removeProductFromCartUseCase;
        _applyCouponUseCase = applyCouponUseCase;
        _removeCouponUseCase = removeCouponUseCase;
        _checkoutCartUseCase = checkoutCartUseCase;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<CartResponse>> Create(
        CancellationToken cancellationToken)
    {
        var output = await _createCartUseCase.ExecuteAsync(
            cancellationToken);

        var response = output.ToResponse();

        return CreatedAtAction(
            nameof(GetById),
            new { cartId = response.Id },
            response);
    }

    [HttpGet("{cartId:guid}")]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> GetById(
        Guid cartId,
        CancellationToken cancellationToken)
    {
        var output = await _getCartUseCase.ExecuteAsync(
            new GetCartInput(cartId),
            cancellationToken);

        return Ok(output.ToResponse());
    }

    [HttpPost("{cartId:guid}/items")]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CartResponse>> AddItem(
        Guid cartId,
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var output =
            await _addProductToCartUseCase.ExecuteAsync(
                new AddProductToCartInput(
                    CartId: cartId,
                    ProductId: request.ProductId,
                    Quantity: request.Quantity),
                cancellationToken);

        return Ok(output.ToResponse());
    }

    [HttpPut(
        "{cartId:guid}/items/{productId:int:min(1)}")]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CartResponse>>
        ChangeItemQuantity(
            Guid cartId,
            int productId,
            [FromBody] ChangeCartItemQuantityRequest request,
            CancellationToken cancellationToken)
    {
        var output =
            await _changeCartItemQuantityUseCase.ExecuteAsync(
                new ChangeCartItemQuantityInput(
                    CartId: cartId,
                    ProductId: productId,
                    Quantity: request.Quantity),
                cancellationToken);

        return Ok(output.ToResponse());
    }

    [HttpDelete(
        "{cartId:guid}/items/{productId:int:min(1)}")]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CartResponse>> RemoveItem(
        Guid cartId,
        int productId,
        CancellationToken cancellationToken)
    {
        var output =
            await _removeProductFromCartUseCase.ExecuteAsync(
                new RemoveProductFromCartInput(
                    CartId: cartId,
                    ProductId: productId),
                cancellationToken);

        return Ok(output.ToResponse());
    }

    [HttpPut("{cartId:guid}/coupon")]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CartResponse>> ApplyCoupon(
        Guid cartId,
        [FromBody] ApplyCouponRequest request,
        CancellationToken cancellationToken)
    {
        var output = await _applyCouponUseCase.ExecuteAsync(
            new ApplyCouponInput(
                CartId: cartId,
                Code: request.Code),
            cancellationToken);

        return Ok(output.ToResponse());
    }

    [HttpDelete("{cartId:guid}/coupon")]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CartResponse>> RemoveCoupon(
        Guid cartId,
        CancellationToken cancellationToken)
    {
        var output =
            await _removeCouponUseCase.ExecuteAsync(
                new RemoveCouponInput(cartId),
                cancellationToken);

        return Ok(output.ToResponse());
    }

    [HttpPost("{cartId:guid}/checkout")]
    [ProducesResponseType(
        typeof(CartResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CartResponse>> Checkout(
        Guid cartId,
        CancellationToken cancellationToken)
    {
        var output = await _checkoutCartUseCase.ExecuteAsync(
            new CheckoutCartInput(cartId),
            cancellationToken);

        return Ok(output.ToResponse());
    }
}