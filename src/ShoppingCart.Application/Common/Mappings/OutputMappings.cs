using ShoppingCart.Application.Models.Outputs;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.Common.Mappings;

internal static class OutputMappings
{
    internal static ProductOutput ToOutput(
        this Product product)
    {
        return new ProductOutput(
            Id: product.Id,
            Description: product.Description,
            AvailableStock: product.StockQuantity,
            NetPrice: product.NetPrice);
    }

    internal static CartOutput ToOutput(
        this Cart cart)
    {
        var items = cart.Items
            .Select(item => new CartItemOutput(
                Id: item.Id,
                ProductId: item.ProductId,
                ProductDescription: item.Product.Description,
                Quantity: item.Quantity,
                AvailableStock: item.Product.StockQuantity,
                NetUnitPrice: item.UnitPrice,
                TotalPrice: item.TotalPrice))
            .ToArray();

        var appliedCoupon = cart.AppliedCoupon is null
            ? null
            : new CouponOutput(
                Id: cart.AppliedCoupon.Id,
                Code: cart.AppliedCoupon.Code,
                DiscountPercentage:
                    cart.AppliedCoupon.DiscountPercentage);

        return new CartOutput(
            Id: cart.Id,
            Status: cart.Status.ToString(),
            Items: items,
            AppliedCoupon: appliedCoupon,
            Subtotal: cart.Subtotal,
            Discount: cart.Discount,
            Total: cart.Total);
    }
}