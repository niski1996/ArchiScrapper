namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Writes payload content to storage under a payload reference.
/// </summary>
public interface IPayloadStorageWriter
{
    /// <summary>
    /// Stores payload content under a given reference.
    /// </summary>
    /// <param name="payloadReference">Storage reference key.</param>
    /// <param name="payload">Payload content to store.</param>
    void PutPayload(string payloadReference, string payload);
}