using HermesTransport.Brokers;
using HermesTransport.Configuration;

namespace HermesTransport.Messaging;

/// <summary>
/// A message publisher that routes messages to the appropriate broker based on message type.
/// </summary>
internal class RoutingMessagePublisher : IMessagePublisher
{
    private readonly IBrokerRegistry _brokerRegistry;

    public RoutingMessagePublisher(IBrokerRegistry brokerRegistry)
    {
        _brokerRegistry = brokerRegistry ?? throw new ArgumentNullException(nameof(brokerRegistry));
    }

    /// <inheritdoc />
    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        var broker = _brokerRegistry.GetBrokerForMessage<TMessage>();
        if (broker == null)
        {
            throw new InvalidOperationException($"No broker registered for message type: {typeof(TMessage).Name}");
        }

        var publisher = broker.GetPublisher();
        return publisher.PublishAsync(message, cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        var broker = _brokerRegistry.GetBrokerForMessage<TMessage>();
        if (broker == null)
        {
            throw new InvalidOperationException($"No broker registered for message type: {typeof(TMessage).Name}");
        }

        var publisher = broker.GetPublisher();
        return publisher.PublishAsync(topic, message, cancellationToken);
    }
}

/// <summary>
/// An event publisher that routes events to the event broker.
/// </summary>
internal class RoutingEventPublisher : IEventPublisher
{
    private readonly IBrokerRegistry _brokerRegistry;

    public RoutingEventPublisher(IBrokerRegistry brokerRegistry)
    {
        _brokerRegistry = brokerRegistry ?? throw new ArgumentNullException(nameof(brokerRegistry));
    }

    /// <inheritdoc />
    public Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IEvent
    {
        var broker = _brokerRegistry.GetEventBroker();
        if (broker == null)
        {
            throw new InvalidOperationException("No event broker registered. Use RegisterEventBroker() to register an event broker.");
        }

        var publisher = broker.GetEventPublisher();
        return publisher.PublishEventAsync(@event, cancellationToken);
    }
}

/// <summary>
/// A command sender that routes commands to the command broker.
/// </summary>
internal class RoutingCommandSender : ICommandSender
{
    private readonly IBrokerRegistry _brokerRegistry;

    public RoutingCommandSender(IBrokerRegistry brokerRegistry)
    {
        _brokerRegistry = brokerRegistry ?? throw new ArgumentNullException(nameof(brokerRegistry));
    }

    /// <inheritdoc />
    public Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand
    {
        var broker = _brokerRegistry.GetCommandBroker();
        if (broker == null)
        {
            throw new InvalidOperationException("No command broker registered. Use RegisterCommandBroker() to register a command broker.");
        }

        var sender = broker.GetCommandSender();
        return sender.SendCommandAsync(command, cancellationToken);
    }
}