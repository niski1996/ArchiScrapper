namespace ArchiScrapper.Messaging.Abstractions;

/// <summary>
/// Builds a full raw-event processing flow from materialization and handling components.
/// </summary>
/// <typeparam name="TPayload">Target payload type.</typeparam>
public interface IRawEventProcessingFlowBuilder<TPayload>
{
    /// <summary>
    /// Sets the envelope materializer used by the flow.
    /// </summary>
    /// <param name="materializer">Materializer instance.</param>
    /// <returns>Current builder instance.</returns>
    IRawEventProcessingFlowBuilder<TPayload> UseMaterializer(IEnvelopeMaterializer materializer);

    /// <summary>
    /// Adds an infrastructure step to the handling section.
    /// </summary>
    /// <param name="infrastructureMiddleware">Infrastructure step instance.</param>
    /// <returns>Current builder instance.</returns>
    IRawEventProcessingFlowBuilder<TPayload> UseInfrastructureStep(IInfrastructureStep<TPayload> infrastructureMiddleware);

    /// <summary>
    /// Adds a business step to the handling section.
    /// </summary>
    /// <param name="businessMiddleware">Business step instance.</param>
    /// <returns>Current builder instance.</returns>
    IRawEventProcessingFlowBuilder<TPayload> UseBusinessStep(IBusinessStep<TPayload> businessMiddleware);

    /// <summary>
    /// Sets the final consumer handler.
    /// </summary>
    /// <param name="consumer">Consumer instance.</param>
    /// <returns>Current builder instance.</returns>
    IRawEventProcessingFlowBuilder<TPayload> UseHandler(IEventConsumer<TPayload> consumer);

    /// <summary>
    /// Sets the central handling error handler.
    /// </summary>
    /// <param name="errorHandler">Error handler instance.</param>
    /// <returns>Current builder instance.</returns>
    IRawEventProcessingFlowBuilder<TPayload> UseErrorHandler(IHandlingPipelineErrorHandler<TPayload> errorHandler);

    /// <summary>
    /// Builds the configured raw event processing flow.
    /// </summary>
    /// <returns>Configured flow instance.</returns>
    IRawEventProcessingFlow<TPayload> Build();
}