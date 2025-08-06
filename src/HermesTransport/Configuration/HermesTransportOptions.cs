using HermesTransport.Brokers;
using HermesTransport.Messaging;

namespace HermesTransport.Configuration;

/// <summary>
/// Configuration options for HermesTransport when used with dependency injection.
/// Provides a fluent API for registering different brokers for different message types.
/// </summary>
public class HermesTransportOptions
{
    internal IBrokerRegistry Registry { get; } = new BrokerRegistry();

    /// <summary>
    /// Registers a message broker for handling events.
    /// </summary>
    /// <param name="broker">The broker to register for events.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions RegisterEventBroker(IMessageBroker broker)
    {
        Registry.RegisterEventBroker(broker);
        return this;
    }

    /// <summary>
    /// Registers a message broker for handling commands.
    /// </summary>
    /// <param name="broker">The broker to register for commands.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions RegisterCommandBroker(IMessageBroker broker)
    {
        Registry.RegisterCommandBroker(broker);
        return this;
    }

    /// <summary>
    /// Registers a message broker for handling general messages.
    /// </summary>
    /// <param name="broker">The broker to register for messages.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HermesTransportOptions RegisterMessageBroker(IMessageBroker broker)
    {
        Registry.RegisterMessageBroker(broker);
        return this;
    }
}