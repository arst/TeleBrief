using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.SqliteVec;
using TeleBrief.Infrastructure.Data;

namespace TeleBrief.Infrastructure.Memory;

public class FactStore
{
    private const string CollectionName = "hourly_summaries";
    private readonly SqliteVectorStore _sqliteVectorStore;

    [Experimental("SKEXP0070")]
    public FactStore(AppConfig config)
    {
        var embeddingGenerator = new GoogleAIEmbeddingGenerator("text-embedding-004", config.Gemini.Key);

        _sqliteVectorStore = new SqliteVectorStore(Db.ConnectionString, new SqliteVectorStoreOptions
        {
            EmbeddingGenerator = embeddingGenerator
        });
    }

    public async Task<List<Fact>> GetFacts(string query, CancellationToken cancellationToken = default)
    {
        var factCollection = _sqliteVectorStore.GetCollection<string, Fact>(CollectionName);
        await factCollection.EnsureCollectionExistsAsync(cancellationToken);
        var facts = factCollection.SearchAsync(query, 20, new VectorSearchOptions<Fact>
        {
            IncludeVectors = false
        }, cancellationToken);

        var result = new List<Fact>();

        await foreach (var fact in facts) result.Add(fact.Record);

        return result;
    }

    public async Task AddFact(Fact fact, CancellationToken cancellationToken = default)
    {
        var collection = _sqliteVectorStore.GetCollection<string, Fact>(CollectionName);
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(fact, cancellationToken);
    }
}