using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Application.Common.Exceptions;
using ShoppingCart.Application.Common.Mappings;
using ShoppingCart.Application.Models.Inputs;
using ShoppingCart.Application.Models.Outputs;

namespace ShoppingCart.Application.UseCases.Carts;

public sealed class GetCartUseCase
{
    private readonly ICartRepository _cartRepository;

    public GetCartUseCase(
        ICartRepository cartRepository)
    {
        _cartRepository = cartRepository
            ?? throw new ArgumentNullException(
                nameof(cartRepository));
    }

    public async Task<CartOutput> ExecuteAsync(
        GetCartInput input,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByIdAsync(
            input.CartId,
            cancellationToken);

        if (cart is null)
        {
            throw new NotFoundException(
                $"O carrinho '{input.CartId}' não foi encontrado.");
        }

        return cart.ToOutput();
    }
}