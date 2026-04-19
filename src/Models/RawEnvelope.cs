using ArchiScrapper.Contracts;

namespace ArchiScrapper.Models;

public record RawEnvelope(
    string FirstName,
    string LastName,
    string City,
    string Payload,
    string? PayloadReference = null)
    : EventEnvelopeBase(FirstName, LastName, City), IHasPayload, IHasPayloadReference;