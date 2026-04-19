using System.Text.Json;

namespace ArchiScrapper.Models;

public sealed record SamplePayload
{
    public SamplePayload(IReadOnlyList<SamplePayloadItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        Items = items;
    }

    public IReadOnlyList<SamplePayloadItem> Items { get; }

    public string ToPayloadString()
    {
        return JsonSerializer.Serialize(Items);
    }
}
