using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopePublicationReferenceTests
{
    [Fact]
    public void ComposeWithReferenceStoresPayloadAndCreatesReferenceEnvelope()
    {
        var storage = new InMemoryPayloadStorageProvider();
        var pipeline = new EnvelopePublicationPipeline(storage);
        var source = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        var result = pipeline.ComposeWithReference(
            source,
            payload => System.Text.Json.JsonSerializer.Serialize(payload),
            "payload-1");

        Assert.Equal(source.FirstName, result.FirstName);
        Assert.Equal(source.LastName, result.LastName);
        Assert.Equal(source.City, result.City);
        Assert.Equal(string.Empty, result.Payload);
        Assert.Equal("payload-1", result.PayloadReference);
        Assert.Equal("{\"FirstName\":\"Jan\",\"LastName\":\"Kowalski\",\"City\":\"Krakow\"}", storage.GetPayload("payload-1"));
    }

    [Fact]
    public void ComposeWithReferenceThrowsWhenReferenceIsInvalid()
    {
        var storage = new InMemoryPayloadStorageProvider();
        var pipeline = new EnvelopePublicationPipeline(storage);
        var source = new TypedEnvelope<PersonPayload>(
            "Jan",
            "Kowalski",
            "Krakow",
            new PersonPayload("Jan", "Kowalski", "Krakow"));

        Assert.Throws<ArgumentException>(() => pipeline.ComposeWithReference(
            source,
            payload => System.Text.Json.JsonSerializer.Serialize(payload),
            string.Empty));
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);
}