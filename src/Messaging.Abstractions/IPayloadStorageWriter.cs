namespace ArchiScrapper.Messaging.Abstractions;

public interface IPayloadStorageWriter
{
    void PutPayload(string payloadReference, string payload);
}