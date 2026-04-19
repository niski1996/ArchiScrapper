using ArchiScrapper.Contracts;

namespace ArchiScrapper.Models;

public sealed record TypedEnvelope<TPayload>(
    string FirstName,
    string LastName,
    string City,
    TPayload Payload)
    : EventEnvelopeBase(FirstName, LastName, City), IHasPayload;