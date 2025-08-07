using HermesTransport.Brokers;
using Microsoft.Extensions.DependencyInjection;

namespace HermesTransport.Configuration;

/// <summary>
///     Configuration options for HermesTransport when used with dependency injection.
///     Provides a fluent API for registering different brokers for different message types.
/// </summary>
public class HermesTransportOptions
{
    internal readonly BrokerRegistryBuilder BrokerRegistryBuilder = new();
    public IServiceCollection Services { get; }

    public HermesTransportOptions(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    ///     Registers a message broker for handling commands.
    /// </summary>
    /// <param name="brokerFactory">The broker factory to register for commands.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions RegisterCommandBroker(Func<IServiceProvider, IMessageBroker> brokerFactory)
    {
        BrokerRegistryBuilder.RegisterCommandBroker(brokerFactory);
        return this;
    }

    /// <summary>
    ///     Registers a message broker for handling events.
    /// </summary>
    /// <param name="brokerFactory">The broker factory to register for events.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions RegisterEventBroker(Func<IServiceProvider, IMessageBroker> brokerFactory)
    {
        BrokerRegistryBuilder.RegisterEventBroker(brokerFactory);
        return this;
    }

    /// <summary>
    ///     Registers a message broker for handling general messages.
    /// </summary>
    /// <param name="brokerFactory">The broker factory to register for messages.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions RegisterMessageBroker(Func<IServiceProvider, IMessageBroker> brokerFactory)
    {
        BrokerRegistryBuilder.RegisterMessageBroker(brokerFactory);
        return this;
    }
}