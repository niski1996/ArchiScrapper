using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class SamplePayloadTests
{
    [Fact]
    public void ToPayloadStringSerializesAllItems()
    {
        var payload = new SamplePayload(
            new List<SamplePayloadItem>
            {
                new("Jan", "Kowalski", "4A", "Krakow"),
                new("Anna", "Nowak", "5B", "Gdansk"),
            });

        var rawPayload = payload.ToPayloadString();

        Assert.Contains("Jan", rawPayload, StringComparison.Ordinal);
        Assert.Contains("Kowalski", rawPayload, StringComparison.Ordinal);
        Assert.Contains("4A", rawPayload, StringComparison.Ordinal);
        Assert.Contains("Krakow", rawPayload, StringComparison.Ordinal);
        Assert.Contains("Anna", rawPayload, StringComparison.Ordinal);
        Assert.Contains("Nowak", rawPayload, StringComparison.Ordinal);
        Assert.Contains("5B", rawPayload, StringComparison.Ordinal);
        Assert.Contains("Gdansk", rawPayload, StringComparison.Ordinal);
    }
}
