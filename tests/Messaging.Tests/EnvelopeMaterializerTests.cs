using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopeMaterializerTests
{
    [Fact]
    public void MaterializeCreatesTypedEnvelopeWithCopiedMetadata()
    {
        var source = new ResolvingExampleEvent(
            "Jan",
            "Kowalski",
            "Krakow",
            "{\"FirstName\":\"Jan\",\"LastName\":\"Kowalski\",\"City\":\"Krakow\"}");

        var materializer = new global::ArchiScrapper.Messaging.Core.EnvelopeMaterializer();

        var result = materializer.Materialize(
            source,
            rawPayload => System.Text.Json.JsonSerializer.Deserialize<PersonPayload>(rawPayload)!);

        Assert.Equal(source.FirstName, result.FirstName);
        Assert.Equal(source.LastName, result.LastName);
        Assert.Equal(source.City, result.City);
        Assert.Equal("Jan", result.Payload.FirstName);
        Assert.Equal("Kowalski", result.Payload.LastName);
        Assert.Equal("Krakow", result.Payload.City);
    }

    private sealed record PersonPayload(string FirstName, string LastName, string City);
}