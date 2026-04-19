using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EnvelopeBaseTests
{
    [Fact]
    public void EventEnvelopeBaseCarriesThreeFields()
    {
        var envelope = new ExampleEnvelope("Jan", "Kowalski", "Krakow");

        Assert.Equal("Jan", envelope.FirstName);
        Assert.Equal("Kowalski", envelope.LastName);
        Assert.Equal("Krakow", envelope.City);
    }

    [Fact]
    public void ResolvingExampleEventImplementsPayloadMarkerInterfaces()
    {
        var example = new ResolvingExampleEvent("Anna", "Nowak", "Gdansk", "payload", "ref-1");

        Assert.IsAssignableFrom<ArchiScrapper.Contracts.IHasPayload>(example);
        Assert.IsAssignableFrom<ArchiScrapper.Contracts.IHasPayloadReference>(example);
        Assert.Equal("payload", example.Payload);
        Assert.Equal("ref-1", example.PayloadReference);
    }

    private sealed record ExampleEnvelope(string FirstName, string LastName, string City)
        : EventEnvelopeBase(FirstName, LastName, City);
}
