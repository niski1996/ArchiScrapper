namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Action selected by handling error policy when a pipeline step fails.
/// </summary>
public enum HandlingPipelineErrorAction
{
    /// <summary>
    /// Retry the same step.
    /// </summary>
    Retry = 0,

    /// <summary>
    /// Continue with next pipeline step.
    /// </summary>
    Continue = 1,

    /// <summary>
    /// Stop processing and rethrow error.
    /// </summary>
    Stop = 2,
}