using HermesTransport.Brokers;
using HermesTransport.Discovery;
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

    /// <summary>
    /// Gets the handler discovery options.
    /// </summary>
    public HandlerDiscoveryOptions HandlerDiscoveryOptions { get; } = new();

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

    /// <summary>
    /// Enables handler autodiscovery with the specified configuration.
    /// </summary>
    /// <param name="configure">Action to configure handler discovery options.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions EnableHandlerDiscovery(Action<HandlerDiscoveryOptions>? configure = null)
    {
        HandlerDiscoveryOptions.IsEnabled = true;
        configure?.Invoke(HandlerDiscoveryOptions);
        return this;
    }

    /// <summary>
    /// Enables handler autodiscovery and automatically registers discovered handlers in the DI container.
    /// </summary>
    /// <param name="configure">Action to configure handler discovery options.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions EnableHandlerDiscoveryWithAutoRegistration(Action<HandlerDiscoveryOptions>? configure = null)
    {
        HandlerDiscoveryOptions.IsEnabled = true;
        configure?.Invoke(HandlerDiscoveryOptions);

        // Auto-register discovered handlers in DI
        RegisterDiscoveredHandlersInDI();
        
        return this;
    }

    private void RegisterDiscoveredHandlersInDI()
    {
        var discoveryService = new HandlerDiscoveryService();
        var assemblies = HandlerDiscoveryOptions.AssembliesToScan.ToArray();
        
        if (assemblies.Length > 0)
        {
            var registrations = discoveryService.DiscoverHandlers(assemblies, HandlerDiscoveryOptions.TypeFilter);
            
            foreach (var registration in registrations)
            {
                // Register handler as transient in DI container
                Services.AddTransient(registration.HandlerType);
            }
        }
    }
}