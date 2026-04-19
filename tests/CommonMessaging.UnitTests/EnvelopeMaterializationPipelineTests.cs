using System.Text.Json;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopeMaterializationPipelineTests
{
    [Fact]
    public void MaterializeCreatesTypedEnvelope()
    {
        var rawPayload = JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow"));

        var source = new RawEnvelope("Jan", "Kowalski", "Krakow", rawPayload);
        var pipeline = new global::ArchiScrapper.Messaging.Core.EnvelopeMaterializationPipeline();

        var result = pipeline.Materialize(
            source,
            payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        Assert.Equal(source.FirstName, result.FirstName);
        Assert.Equal(source.LastName, result.LastName);
        Assert.Equal(source.City, result.City);
        Assert.Equal("Jan", result.Payload.FirstName);
        Assert.Equal("Kowalski", result.Payload.LastName);
        Assert.Equal("Krakow", result.Payload.City);
    }

    [Fact]
    public void MaterializeUsesPayloadReferenceWhenResolverFindsExternalPayload()
    {
        var externalPayload = JsonSerializer.Serialize(new PersonPayload("Anna", "Nowak", "Gdansk"));

        var storage = new global::ArchiScrapper.Messaging.Core.InMemoryPayloadStorageProvider(
            new Dictionary<string, string>
            {
                ["ref-42"] = externalPayload,
            });

        var resolver = new global::ArchiScrapper.Messaging.Core.PayloadSourceResolver(storage);
        var pipeline = new global::ArchiScrapper.Messaging.Core.EnvelopeMaterializationPipeline(resolver);

        var source = new RawEnvelope("Anna", "Nowak", "Gdansk", "inline-payload", "ref-42");

        var result = pipeline.Materialize(
            source,
            payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);

        Assert.Equal("Anna", result.Payload.FirstName);
        Assert.Equal("Nowak", result.Payload.LastName);
        Assert.Equal("Gdansk", result.Payload.City);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);
}