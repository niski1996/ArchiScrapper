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

        var source = new ResolvingExampleEvent("Jan", "Kowalski", "Krakow", rawPayload, "ref-1");
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

    private sealed record PersonPayload(string FirstName, string LastName, string City);
}