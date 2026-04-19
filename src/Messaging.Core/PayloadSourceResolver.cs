using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

/// <summary>
/// Default payload resolver that reads inline payload or resolves payload by reference from storage.
/// </summary>
public sealed class PayloadSourceResolver : IPayloadSourceResolver
{
    private readonly IPayloadStorageProvider storageProvider;

    /// <summary>
    /// Initializes a resolver with storage provider used for payload references.
    /// </summary>
    /// <param name="storageProvider">Storage provider used to resolve referenced payloads.</param>
    public PayloadSourceResolver(IPayloadStorageProvider storageProvider)
    {
        this.storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
    }

    /// <inheritdoc />
    public string ResolvePayload(RawEnvelope source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!string.IsNullOrWhiteSpace(source.PayloadReference))
        {
            return storageProvider.GetPayload(source.PayloadReference);
        }

        return source.Payload;
    }
}