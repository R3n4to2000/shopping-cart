using ShoppingCart.Application.UseCases.Carts;
using ShoppingCart.Application.UseCases.Products;

namespace ShoppingCart.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationUseCases(
        this IServiceCollection services)
    {
        services.AddScoped<ListProductsUseCase>();

        services.AddScoped<CreateCartUseCase>();
        services.AddScoped<GetCartUseCase>();
        services.AddScoped<AddProductToCartUseCase>();
        services.AddScoped<ChangeCartItemQuantityUseCase>();
        services.AddScoped<RemoveProductFromCartUseCase>();
        services.AddScoped<ApplyCouponUseCase>();
        services.AddScoped<RemoveCouponUseCase>();
        services.AddScoped<CheckoutCartUseCase>();

        return services;
    }
}