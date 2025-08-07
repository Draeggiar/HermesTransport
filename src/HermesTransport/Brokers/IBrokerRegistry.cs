using HermesTransport.Messaging;
using HermesTransport.Subscriptions;

namespace HermesTransport.Brokers;

/// <summary>
/// Defines operations for managing multiple message brokers with different subscription options.
/// Allows registering different brokers for different message types (Events, Commands, Messages).  
/// </summary>
internal interface IBrokerRegistry
{
    /// <summary>
    /// Gets the registered broker for events.
    /// </summary>
    /// <returns>The event broker if registered, otherwise null.</returns>
    IMessageBroker? GetEventBroker();
    
    /// <summary>
    /// Gets the registered broker for commands.
    /// </summary>
    /// <returns>The command broker if registered, otherwise null.</returns>
    IMessageBroker? GetCommandBroker();
    
    /// <summary>
    /// Gets the registered broker for messages.
    /// </summary>
    /// <returns>The message broker if registered, otherwise null.</returns>
    IMessageBroker? GetMessageBroker();
    
    /// <summary>
    /// Gets a composite message publisher that routes messages to appropriate brokers.
    /// </summary>
    /// <returns>A message publisher that handles routing.</returns>
    IMessagePublisher GetPublisher();
    
    /// <summary>
    /// Gets a composite message subscriber that can subscribe across all registered brokers.
    /// </summary>
    /// <returns>A message subscriber that handles routing.</returns>
    IMessageSubscriber GetSubscriber();
    
    /// <summary>
    /// Gets a composite event publisher that routes events to the event broker.
    /// </summary>
    /// <returns>An event publisher that handles routing.</returns>
    IEventPublisher GetEventPublisher();
    
    /// <summary>
    /// Gets a composite command sender that routes commands to the command broker.
    /// </summary>
    /// <returns>A command sender that handles routing.</returns>
    ICommandSender GetCommandSender();
}