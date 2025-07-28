namespace Hermes.Core;

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

    /// <summary>
    /// Publishes a message envelope to the messaging system.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="envelope">The message envelope to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage, TContext>(IMessageEnvelope<TMessage, TContext> envelope, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;

    /// <summary>
    /// Publishes a message envelope to a specific topic or channel.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="topic">The topic or channel to publish to.</param>
    /// <param name="envelope">The message envelope to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage, TContext>(string topic, IMessageEnvelope<TMessage, TContext> envelope, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;

    /// <summary>
    /// Publishes a message with context information to the messaging system.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="context">The context information to include.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage, TContext>(TMessage message, TContext context, CancellationToken cancellationToken = default) 
        where TMessage : IMessage;

    /// <summary>
    /// Publishes a message with context information to a specific topic or channel.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="topic">The topic or channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="context">The context information to include.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage, TContext>(string topic, TMessage message, TContext context, CancellationToken cancellationToken = default) 
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

    /// <summary>
    /// Publishes an event with context information to all subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="event">The event to publish.</param>
    /// <param name="context">The context information to include.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishEventAsync<TEvent, TContext>(TEvent @event, TContext context, CancellationToken cancellationToken = default) 
        where TEvent : IEvent;

    /// <summary>
    /// Publishes an event envelope to all subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="envelope">The event envelope to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishEventAsync<TEvent, TContext>(IMessageEnvelope<TEvent, TContext> envelope, CancellationToken cancellationToken = default) 
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

    /// <summary>
    /// Sends a command with context information to be processed.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to send.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="context">The context information to include.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendCommandAsync<TCommand, TContext>(TCommand command, TContext context, CancellationToken cancellationToken = default) 
        where TCommand : ICommand;

    /// <summary>
    /// Sends a command envelope to be processed.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to send.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="envelope">The command envelope to send.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendCommandAsync<TCommand, TContext>(IMessageEnvelope<TCommand, TContext> envelope, CancellationToken cancellationToken = default) 
        where TCommand : ICommand;
}