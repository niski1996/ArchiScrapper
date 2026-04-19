using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;

namespace ArchiScrapper.Messaging.Extensions;

public static class EnvelopePublicationPolicyBuilderExtensions
{
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