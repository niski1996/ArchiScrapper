namespace ArchiScrapper.Contracts;

public interface IEvent
{
	Guid Id { get; }

	string EventType { get; }

	DateTimeOffset CreationDate { get; }

	long Timestamp { get; }
}
