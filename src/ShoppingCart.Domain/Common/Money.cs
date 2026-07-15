namespace ShoppingCart.Domain.Common;

internal static class Money
{
    public static decimal Round(decimal value)
    {
        return Math.Round(
            value,
            2,
            MidpointRounding.AwayFromZero);
    }
}