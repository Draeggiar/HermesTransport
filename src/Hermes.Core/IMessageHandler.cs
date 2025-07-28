namespace Hermes.Core;

/// <summary>
/// Defines a handler for processing messages of a specific type.
/// </summary>
/// <typeparam name="TMessage">The type of message this handler can process.</typeparam>
public interface IMessageHandler<in TMessage> where TMessage : IMessage
{
    /// <summary>
    /// Handles the specified message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a handler for processing message envelopes with context.
/// </summary>
/// <typeparam name="TMessage">The type of message this handler can process.</typeparam>
/// <typeparam name="TContext">The type of context information.</typeparam>
public interface IMessageEnvelopeHandler<in TMessage, in TContext> where TMessage : IMessage
{
    /// <summary>
    /// Handles the specified message envelope.
    /// </summary>
    /// <param name="envelope">The message envelope to handle.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(IMessageEnvelope<TMessage, TContext> envelope, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a handler for processing message envelopes without specific context type constraints.
/// </summary>
/// <typeparam name="TMessage">The type of message this handler can process.</typeparam>
public interface IMessageEnvelopeHandler<in TMessage> : IMessageEnvelopeHandler<TMessage, object?> where TMessage : IMessage
{
}

/// <summary>
/// Defines a handler for processing events of a specific type.
/// </summary>
/// <typeparam name="TEvent">The type of event this handler can process.</typeparam>
public interface IEventHandler<in TEvent> : IMessageHandler<TEvent> where TEvent : IEvent
{
}

/// <summary>
/// Defines a handler for processing event envelopes with context.
/// </summary>
/// <typeparam name="TEvent">The type of event this handler can process.</typeparam>
/// <typeparam name="TContext">The type of context information.</typeparam>
public interface IEventEnvelopeHandler<in TEvent, in TContext> : IMessageEnvelopeHandler<TEvent, TContext> where TEvent : IEvent
{
}

/// <summary>
/// Defines a handler for processing event envelopes without specific context type constraints.
/// </summary>
/// <typeparam name="TEvent">The type of event this handler can process.</typeparam>
public interface IEventEnvelopeHandler<in TEvent> : IMessageEnvelopeHandler<TEvent> where TEvent : IEvent
{
}

/// <summary>
/// Defines a handler for processing commands of a specific type.
/// </summary>
/// <typeparam name="TCommand">The type of command this handler can process.</typeparam>
public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand> where TCommand : ICommand
{
}

/// <summary>
/// Defines a handler for processing command envelopes with context.
/// </summary>
/// <typeparam name="TCommand">The type of command this handler can process.</typeparam>
/// <typeparam name="TContext">The type of context information.</typeparam>
public interface ICommandEnvelopeHandler<in TCommand, in TContext> : IMessageEnvelopeHandler<TCommand, TContext> where TCommand : ICommand
{
}

/// <summary>
/// Defines a handler for processing command envelopes without specific context type constraints.
/// </summary>
/// <typeparam name="TCommand">The type of command this handler can process.</typeparam>
public interface ICommandEnvelopeHandler<in TCommand> : IMessageEnvelopeHandler<TCommand> where TCommand : ICommand
{
}