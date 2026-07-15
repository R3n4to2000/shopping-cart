using ShoppingCart.Api.Contracts.Carts;
using ShoppingCart.Api.Contracts.Products;
using ShoppingCart.Application.Models.Outputs;

namespace ShoppingCart.Api.Mappings;

internal static class ApiResponseMappings
{
    internal static ProductResponse ToResponse(
        this ProductOutput output)
    {
        return new ProductResponse(
            Id: output.Id,
            Description: output.Description,
            AvailableStock: output.AvailableStock,
            NetPrice: output.NetPrice);
    }

    internal static CartResponse ToResponse(
        this CartOutput output)
    {
        var items = output.Items
            .Select(item => new CartItemResponse(
                Id: item.Id,
                ProductId: item.ProductId,
                ProductDescription: item.ProductDescription,
                Quantity: item.Quantity,
                AvailableStock: item.AvailableStock,
                NetUnitPrice: item.NetUnitPrice,
                TotalPrice: item.TotalPrice))
            .ToArray();

        var coupon = output.AppliedCoupon is null
            ? null
            : new CouponResponse(
                Id: output.AppliedCoupon.Id,
                Code: output.AppliedCoupon.Code,
                DiscountPercentage:
                    output.AppliedCoupon.DiscountPercentage);

        return new CartResponse(
            Id: output.Id,
            Status: output.Status,
            Items: items,
            AppliedCoupon: coupon,
            Subtotal: output.Subtotal,
            Discount: output.Discount,
            Total: output.Total);
    }
}