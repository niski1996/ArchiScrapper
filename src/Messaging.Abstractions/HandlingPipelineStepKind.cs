namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Handling pipeline step groups used by error policy and diagnostics.
/// </summary>
public enum HandlingPipelineStepKind
{
    /// <summary>
    /// Infrastructure step group.
    /// </summary>
    Infrastructure = 0,

    /// <summary>
    /// Business step group.
    /// </summary>
    Business = 1,

    /// <summary>
    /// Final consumer step.
    /// </summary>
    Consumer = 2,
}