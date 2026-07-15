using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Application.Common.Exceptions;
using ShoppingCart.Application.Common.Mappings;
using ShoppingCart.Application.Models.Inputs;
using ShoppingCart.Application.Models.Outputs;

namespace ShoppingCart.Application.UseCases.Carts;

public sealed class ChangeCartItemQuantityUseCase
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeCartItemQuantityUseCase(
        ICartRepository cartRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository
            ?? throw new ArgumentNullException(
                nameof(cartRepository));

        _unitOfWork = unitOfWork
            ?? throw new ArgumentNullException(
                nameof(unitOfWork));
    }

    public async Task<CartOutput> ExecuteAsync(
        ChangeCartItemQuantityInput input,
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

        cart.ChangeProductQuantity(
            input.ProductId,
            input.Quantity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return cart.ToOutput();
    }
}