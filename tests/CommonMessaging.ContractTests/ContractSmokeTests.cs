using System.Text.Json;
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.CommonMessaging.ContractTests;

public class EnvelopeMaterializerContractTests
{
    [Fact]
    public void EnvelopeMaterializerSatisfiesContract()
    {
        var sut = new EnvelopeMaterializer();
        AssertMaterializerContract(sut);
    }

    private static void AssertMaterializerContract<TMaterializer>(TMaterializer sut)
        where TMaterializer : IEnvelopeMaterializer
    {
        var source = new RawEnvelope(
            "Jan",
            "Kowalski",
            "Krakow",
            JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow")));

        var result = sut.Materialize(
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
