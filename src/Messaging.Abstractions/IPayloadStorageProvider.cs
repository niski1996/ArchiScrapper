namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Reads payload content from storage using payload references.
/// </summary>
public interface IPayloadStorageProvider
{
    /// <summary>
    /// Gets payload content by reference.
    /// </summary>
    /// <param name="payloadReference">Storage reference key.</param>
    /// <returns>Stored payload content.</returns>
    string GetPayload(string payloadReference);
}