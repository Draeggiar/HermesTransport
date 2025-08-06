namespace HermesTransport.Messaging;

/// <summary>
/// Defines operations for publishing messages to the messaging system.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message to the messaging system.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;

    /// <summary>
    /// Publishes a message to a specific topic or channel.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <param name="topic">The topic or channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;
}

/// <summary>
/// Defines operations for sending events to the messaging system.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to all subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent;
}

/// <summary>
/// Defines operations for sending commands to the messaging system.
/// </summary>
public interface ICommandSender
{
    /// <summary>
    /// Sends a command to be processed.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to send.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand;
}