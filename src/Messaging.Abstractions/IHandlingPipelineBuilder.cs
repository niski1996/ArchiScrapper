namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Builds an <see cref="IHandlingPipeline{TPayload}"/> by composing infrastructure steps, business steps, and consumer.
/// </summary>
/// <typeparam name="TPayload">Payload type processed by the pipeline.</typeparam>
public interface IHandlingPipelineBuilder<TPayload>
{
    /// <summary>
    /// Adds an infrastructure step to the pipeline.
    /// </summary>
    /// <param name="infrastructureMiddleware">Infrastructure step instance.</param>
    /// <returns>Current builder instance.</returns>
    IHandlingPipelineBuilder<TPayload> UseInfrastructureStep(IInfrastructureStep<TPayload> infrastructureMiddleware);

    /// <summary>
    /// Adds a business step to the pipeline.
    /// </summary>
    /// <param name="businessMiddleware">Business step instance.</param>
    /// <returns>Current builder instance.</returns>
    IHandlingPipelineBuilder<TPayload> UseBusinessStep(IBusinessStep<TPayload> businessMiddleware);

    /// <summary>
    /// Sets the final consumer handler.
    /// </summary>
    /// <param name="consumer">Consumer instance.</param>
    /// <returns>Current builder instance.</returns>
    IHandlingPipelineBuilder<TPayload> UseHandler(IEventConsumer<TPayload> consumer);

    /// <summary>
    /// Sets the central error handler used by pipeline steps.
    /// </summary>
    /// <param name="errorHandler">Error handler instance.</param>
    /// <returns>Current builder instance.</returns>
    IHandlingPipelineBuilder<TPayload> UseErrorHandler(IHandlingPipelineErrorHandler<TPayload> errorHandler);

    /// <summary>
    /// Builds the configured handling pipeline.
    /// </summary>
    /// <returns>Configured pipeline instance.</returns>
    IHandlingPipeline<TPayload> Build();
}