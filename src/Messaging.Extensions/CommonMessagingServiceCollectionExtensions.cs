using ArchiScrapper.Messaging.Abstractions;
using ArchiScrapper.Messaging.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ArchiScrapper.Messaging.Extensions;

/// <summary>
/// Dependency injection registration extensions for Common Messaging components.
/// </summary>
public static class CommonMessagingServiceCollectionExtensions
{
    /// <summary>
    /// Registers core messaging services: payload storage, source resolver, publication pipeline and materialization pipeline.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Same service collection for chaining.</returns>
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

    /// <summary>
    /// Registers raw event processing flow and handling pipeline for payload type <typeparamref name="TPayload"/>.
    /// </summary>
    /// <typeparam name="TPayload">Payload type processed by the registered flow.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <returns>Same service collection for chaining.</returns>
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

    /// <summary>
    /// Registers transport publishing flow for payload type <typeparamref name="TPayload"/>.
    /// </summary>
    /// <typeparam name="TPayload">Payload type published by the registered flow.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <returns>Same service collection for chaining.</returns>
    public static IServiceCollection AddEnvelopeTransportPublishingFlow<TPayload>(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IEnvelopeTransportPublishingFlow<TPayload>, EnvelopeTransportPublishingFlow<TPayload>>();

        return services;
    }
}
