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
/// Defines a handler for processing events of a specific type.
/// </summary>
/// <typeparam name="TEvent">The type of event this handler can process.</typeparam>
public interface IEventHandler<in TEvent> : IMessageHandler<TEvent> where TEvent : IEvent
{
}

/// <summary>
/// Defines a handler for processing commands of a specific type.
/// </summary>
/// <typeparam name="TCommand">The type of command this handler can process.</typeparam>
public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand> where TCommand : ICommand
{
}