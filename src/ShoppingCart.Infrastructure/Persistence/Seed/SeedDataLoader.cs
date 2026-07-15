using System.Reflection;
using System.Text.Json;

namespace ShoppingCart.Infrastructure.Persistence.Seed;

internal static class SeedDataLoader
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    internal static IReadOnlyCollection<T> Load<T>(
        string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "O nome do arquivo de seed é obrigatório.",
                nameof(fileName));
        }

        var assembly = typeof(SeedDataLoader).Assembly;

        var resourceName = assembly
            .GetManifestResourceNames()
            .SingleOrDefault(name =>
                name.EndsWith(
                    fileName,
                    StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            var availableResources = string.Join(
                ", ",
                assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"O recurso de seed '{fileName}' não foi encontrado. " +
                $"Recursos disponíveis: {availableResources}.");
        }

        using var stream =
            assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Não foi possível abrir o recurso de seed '{resourceName}'.");
        }

        var data = JsonSerializer.Deserialize<T[]>(
            stream,
            SerializerOptions);

        if (data is null || data.Length == 0)
        {
            throw new InvalidOperationException(
                $"O arquivo de seed '{fileName}' não contém registros válidos.");
        }

        return data;
    }
}