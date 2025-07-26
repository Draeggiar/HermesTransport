namespace Hermes.Core;

/// <summary>
/// A facade service that provides a unified interface for all messaging operations.
/// This service combines publisher, subscriber, and broker functionality.
/// </summary>
public class HermesMessagingService : IEventPublisher, ICommandSender, IMessageSubscriber, IDisposable
{
    private readonly IMessageBroker _broker;

    public HermesMessagingService(IMessageBroker broker)
    {
        _broker = broker ?? throw new ArgumentNullException(nameof(broker));
    }

    /// <summary>
    /// Gets the underlying message broker instance.
    /// </summary>
    public IMessageBroker Broker => _broker;

    /// <inheritdoc />
    public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent
    {
        await _broker.PublishAsync(@event, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand
    {
        await _broker.PublishAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public ISubscription Subscribe<TMessage>(IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage
    {
        return _broker.Subscribe(handler, options);
    }

    /// <inheritdoc />
    public ISubscription Subscribe<TMessage>(string topic, IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage
    {
        return _broker.Subscribe(topic, handler, options);
    }

    /// <summary>
    /// Publishes a message to a specific topic.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <param name="topic">The topic to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        await _broker.PublishAsync(topic, message, cancellationToken);
    }

    /// <summary>
    /// Publishes a message using its message type as the topic.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to publish.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        await _broker.PublishAsync(message, cancellationToken);
    }

    /// <summary>
    /// Ensures the messaging service is connected and ready.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        if (!_broker.IsConnected)
        {
            await _broker.ConnectAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        _broker?.Dispose();
    }
}