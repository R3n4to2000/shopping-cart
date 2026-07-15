using ShoppingCart.Application.Abstractions.Persistence;
using ShoppingCart.Application.Common.Exceptions;
using ShoppingCart.Application.Common.Mappings;
using ShoppingCart.Application.Models.Inputs;
using ShoppingCart.Application.Models.Outputs;

namespace ShoppingCart.Application.UseCases.Carts;

public sealed class ApplyCouponUseCase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyCouponUseCase(
        ICartRepository cartRepository,
        ICouponRepository couponRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository
            ?? throw new ArgumentNullException(
                nameof(cartRepository));

        _couponRepository = couponRepository
            ?? throw new ArgumentNullException(
                nameof(couponRepository));

        _unitOfWork = unitOfWork
            ?? throw new ArgumentNullException(
                nameof(unitOfWork));
    }

    public async Task<CartOutput> ExecuteAsync(
        ApplyCouponInput input,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            throw new ValidationException(
                "O código do cupom é obrigatório.");
        }

        var cart = await _cartRepository.GetByIdAsync(
            input.CartId,
            cancellationToken);

        if (cart is null)
        {
            throw new NotFoundException(
                $"O carrinho '{input.CartId}' não foi encontrado.");
        }

        var normalizedCode = input.Code
            .Trim()
            .ToUpperInvariant();

        var coupon = await _couponRepository.GetByCodeAsync(
            normalizedCode,
            cancellationToken);

        if (coupon is null)
        {
            throw new NotFoundException(
                $"O cupom '{normalizedCode}' não foi encontrado.");
        }

        cart.ApplyCoupon(coupon);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return cart.ToOutput();
    }
}