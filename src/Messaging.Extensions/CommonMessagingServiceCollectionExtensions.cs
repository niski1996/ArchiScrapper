using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ArchiScrapper.Messaging.Extensions;

public static class CommonMessagingServiceCollectionExtensions
{
    public static IServiceCollection AddCommonMessagingCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IPayloadStorageProvider, InMemoryPayloadStorageProvider>();
        services.AddSingleton<IPayloadStorageWriter>(serviceProvider =>
            (IPayloadStorageWriter)serviceProvider.GetRequiredService<IPayloadStorageProvider>());
        services.AddSingleton<IPayloadSourceResolver, PayloadSourceResolver>();
        services.AddSingleton(typeof(IEnvelopePublicationPolicyBuilder<>), typeof(EnvelopePublicationPolicyBuilder<>));
        services.AddSingleton<IEnvelopePublicationPipeline>(serviceProvider =>
            new EnvelopePublicationPipeline(serviceProvider.GetRequiredService<IPayloadStorageWriter>()));
        services.AddSingleton(typeof(IEnvelopePublisher<>), typeof(EnvelopePublisher<>));
        services.AddSingleton<IEnvelopeMaterializationPipeline>(serviceProvider =>
            new EnvelopeMaterializationPipeline(
                serviceProvider.GetRequiredService<IPayloadSourceResolver>()));
        services.AddSingleton<IEnvelopeMaterializer, EnvelopeMaterializer>();

        return services;
    }

    public static IServiceCollection AddRawEventProcessingFlow<TPayload>(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IHandlingPipeline<TPayload>>(serviceProvider =>
        {
            var infrastructureSteps = serviceProvider.GetServices<IInfrastructureStep<TPayload>>();
            var businessSteps = serviceProvider.GetServices<IBusinessStep<TPayload>>();
            var consumer = serviceProvider.GetRequiredService<IEventConsumer<TPayload>>();
            var errorHandler = serviceProvider.GetService<IHandlingPipelineErrorHandler<TPayload>>();

            return new HandlingPipeline<TPayload>(
                infrastructureSteps.ToArray(),
                businessSteps.ToArray(),
                consumer,
                errorHandler);
        });

        services.AddTransient<IRawEventProcessingFlow<TPayload>>(serviceProvider =>
        {
            var materializer = serviceProvider.GetRequiredService<IEnvelopeMaterializer>();
            var handlingPipeline = serviceProvider.GetRequiredService<IHandlingPipeline<TPayload>>();

            return new RawEventProcessingFlow<TPayload>(materializer, handlingPipeline);
        });

        return services;
    }
}
