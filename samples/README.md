# samples

This folder contains minimal examples of using repository libraries.

## Rules
- Keep examples small and focused.
- Avoid business-specific assumptions in generic samples.
- Ensure examples stay aligned with current public APIs.

## Minimal End-to-End Usage

### 1. Register Services

```csharp
using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services
	.AddCommonMessagingCore()
	.AddRawEventProcessingFlow<PersonPayload>();

services.AddSingleton<IEventConsumer<PersonPayload>, PersonConsumer>();

var serviceProvider = services.BuildServiceProvider();
var flow = serviceProvider.GetRequiredService<IRawEventProcessingFlow<PersonPayload>>();
```

### 2. Inline Payload Scenario

```csharp
using System.Text.Json;
using ArchiScrapper.Models;

var inlineEnvelope = new RawEnvelope(
	FirstName: "Anna",
	LastName: "Nowak",
	City: "Gdansk",
	Payload: JsonSerializer.Serialize(new PersonPayload("Anna", "Nowak", "Gdansk")));

await flow.ProcessAsync(
	inlineEnvelope,
	payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);
```

### 3. Payload Reference Scenario

```csharp
using System.Text.Json;
using ArchiScrapper.Messaging.Core;
using ArchiScrapper.Models;

var storage = (InMemoryPayloadStorageProvider)serviceProvider.GetRequiredService<IPayloadStorageProvider>();
storage.PutPayload("payload-ref-1", JsonSerializer.Serialize(new PersonPayload("Jan", "Kowalski", "Krakow")));

var referencedEnvelope = new RawEnvelope(
	FirstName: "Jan",
	LastName: "Kowalski",
	City: "Krakow",
	Payload: string.Empty,
	PayloadReference: "payload-ref-1");

await flow.ProcessAsync(
	referencedEnvelope,
	payload => JsonSerializer.Deserialize<PersonPayload>(payload)!);
```

### 4. Example Consumer

```csharp
using ArchiScrapper.Messaging.Abstractions;

public sealed record PersonPayload(string FirstName, string LastName, string City);

public sealed class PersonConsumer : IEventConsumer<PersonPayload>
{
	public Task HandleAsync(HandleContext<PersonPayload> context, CancellationToken cancellationToken)
	{
		Console.WriteLine($"Handled: {context.Envelope.Payload.FirstName} {context.Envelope.Payload.LastName}");
		return Task.CompletedTask;
	}
}
```
