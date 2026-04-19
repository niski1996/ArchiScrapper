using ArchiScrapper.Models;
using Xunit;

namespace ArchiScrapper.Messaging.Tests;

public class EventEnvelopeTests
{
    [Fact]
    public void CreateBuildsEnvelopeWithRequiredFields()
    {
        var envelope = EventEnvelope.Create("StudentImported", "sample-payload");

        Assert.NotEqual(Guid.Empty, envelope.Id);
        Assert.Equal("StudentImported", envelope.EventType);
        Assert.NotEqual(default, envelope.CreationDate);
        Assert.True(envelope.Timestamp > 0);
        Assert.Equal("sample-payload", envelope.Payload);
        Assert.Null(envelope.PayloadReference);
    }

    [Fact]
    public void ConstructorStoresPayloadReferenceWhenProvided()
    {
        var now = DateTimeOffset.UtcNow;
        var envelope = new EventEnvelope(
            Guid.NewGuid(),
            "StudentImported",
            now,
            now.ToUnixTimeMilliseconds(),
            "sample-payload",
            "s3://bucket/payload-1");

        Assert.Equal("s3://bucket/payload-1", envelope.PayloadReference);
    }
}
