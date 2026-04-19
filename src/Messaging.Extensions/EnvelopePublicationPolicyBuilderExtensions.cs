using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;

namespace ArchiScrapper.Messaging.Extensions;

/// <summary>
/// Extensions for configuring publication policy builders.
/// </summary>
public static class EnvelopePublicationPolicyBuilderExtensions
{
    /// <summary>
    /// Applies the default publication profile with bounded retries and optional store-step fallback.
    /// </summary>
    /// <typeparam name="TPayload">Payload type being published.</typeparam>
    /// <param name="builder">Policy builder instance.</param>
    /// <param name="maxRetriesPerStep">Maximum retry attempts for each failed step.</param>
    /// <param name="continueOnStoreFailure">Whether store-step failures should continue with inline fallback.</param>
    /// <returns>Same builder instance for chaining.</returns>
    public static IEnvelopePublicationPolicyBuilder<TPayload> UseDefaultProfile<TPayload>(
        this IEnvelopePublicationPolicyBuilder<TPayload> builder,
        int maxRetriesPerStep = 1,
        bool continueOnStoreFailure = true)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.UseErrorHandler(
            new DefaultEnvelopePublicationErrorHandler<TPayload>(
                maxRetriesPerStep,
                continueOnStoreFailure));
    }
}