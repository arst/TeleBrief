using Microsoft.Extensions.VectorData;

namespace TeleBrief.Infrastructure.Memory;

public class Fact
{
    [VectorStoreKey] public required string Id { get; set; }

    public required DateOnly Date { get; init; }

    [VectorStoreData] public required string Category { get; init; } = string.Empty;

    [VectorStoreData] public required string Text { get; init; } = string.Empty;

    [VectorStoreVector(768, DistanceFunction = DistanceFunction.CosineDistance)]
    public string Embedding => Text;
}