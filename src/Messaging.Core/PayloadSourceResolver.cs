using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Core;

public sealed class PayloadSourceResolver : IPayloadSourceResolver
{
    private readonly IPayloadStorageProvider storageProvider;

    public PayloadSourceResolver(IPayloadStorageProvider storageProvider)
    {
        this.storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
    }

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