using ArchiScrapper.Models;

namespace ArchiScrapper.Messaging.Abstractions;

public interface IPayloadSourceResolver
{
    string ResolvePayload(RawEnvelope source);
}