using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Application.Common.Mappings;
using ShoppingCart.Application.Models.Outputs;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.UseCases.Carts;

public sealed class CreateCartUseCase
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCartUseCase(
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
        CancellationToken cancellationToken = default)
    {
        var cart = new Cart();

        await _cartRepository.AddAsync(
            cart,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return cart.ToOutput();
    }
}