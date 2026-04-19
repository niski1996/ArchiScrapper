namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Represents a single stage of the envelope materialization pipeline.
/// </summary>
public interface IEnvelopeMaterializationStage
{
    /// <summary>
    /// Executes the stage logic against a materialization context.
    /// </summary>
    /// <typeparam name="TPayload">Target payload type.</typeparam>
    /// <param name="context">Materialization context carrying source, state, and result.</param>
    void Execute<TPayload>(EnvelopeMaterializationContext<TPayload> context);
}