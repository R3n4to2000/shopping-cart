using System.Text.Json.Serialization;

namespace ShoppingCart.Infrastructure.Persistence.Seed.Models;

internal sealed class ProductSeedModel
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("descricaoProduto")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("quantidadeEstoque")]
    public int StockQuantity { get; init; }

    [JsonPropertyName("precoLiquido")]
    public decimal NetPrice { get; init; }
}