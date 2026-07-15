using System.Text.Json.Serialization;

namespace ShoppingCart.Infrastructure.Persistence.Seed.Models;

internal sealed class CouponSeedModel
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("codigoCupom")]
    public string Code { get; init; } = string.Empty;

    [JsonPropertyName("percentualDesconto")]
    public decimal DiscountPercentage { get; init; }
}