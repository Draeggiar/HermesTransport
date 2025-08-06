using HermesTransport.Messaging;

namespace HermesTransport.Brokers;

/// <summary>
///     Extension methods for configuring BrokerRegistry with a fluent API.
/// </summary>
internal static class BrokerRegistryExtensions
{
    /// <summary>
    ///     Gets the appropriate broker for the specified message type.
    ///     Routes events to the event broker, commands to the command broker, and other messages to the message broker.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="brokerRegistry">The broker registry instance.</param>
    /// <returns>The corresponding <see cref="IMessageBroker" /> for the message type, or null if not found.</returns>
    public static IMessageBroker? GetBrokerForMessage<TMessage>(this IBrokerRegistry brokerRegistry) where TMessage : IMessage
    {
        // Route events to event broker
        if (typeof(IEvent).IsAssignableFrom(typeof(TMessage))) return brokerRegistry.GetEventBroker();

        // Route commands to command broker
        if (typeof(ICommand).IsAssignableFrom(typeof(TMessage))) return brokerRegistry.GetCommandBroker();

        // Route general messages to message broker
        return brokerRegistry.GetMessageBroker();
    }
}