namespace ArchiScrapper.Messaging.Abstractions;

public interface IPayloadStorageProvider
{
    string GetPayload(string payloadReference);
}