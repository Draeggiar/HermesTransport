namespace HermesTransport;

/// <summary>
/// Default implementation of IBrokerRegistry that manages multiple message brokers.
/// </summary>
public class BrokerRegistry : IBrokerRegistry
{
    private IMessageBroker? _eventBroker;
    private IMessageBroker? _commandBroker; 
    private IMessageBroker? _messageBroker;

    /// <inheritdoc />
    public IBrokerRegistry RegisterEventBroker(IMessageBroker broker)
    {
        _eventBroker = broker ?? throw new ArgumentNullException(nameof(broker));
        return this;
    }

    /// <inheritdoc />
    public IBrokerRegistry RegisterCommandBroker(IMessageBroker broker)
    {
        _commandBroker = broker ?? throw new ArgumentNullException(nameof(broker));
        return this;
    }

    /// <inheritdoc />
    public IBrokerRegistry RegisterMessageBroker(IMessageBroker broker)
    {
        _messageBroker = broker ?? throw new ArgumentNullException(nameof(broker));
        return this;
    }

    /// <inheritdoc />
    public IMessageBroker? GetEventBroker() => _eventBroker;

    /// <inheritdoc />
    public IMessageBroker? GetCommandBroker() => _commandBroker;

    /// <inheritdoc />
    public IMessageBroker? GetMessageBroker() => _messageBroker;

    /// <inheritdoc />
    public IMessagePublisher GetPublisher()
    {
        return new RoutingMessagePublisher(this);
    }

    /// <inheritdoc />
    public IMessageSubscriber GetSubscriber()
    {
        return new RoutingMessageSubscriber(this);
    }

    /// <inheritdoc />
    public IEventPublisher GetEventPublisher()
    {
        return new RoutingEventPublisher(this);
    }

    /// <inheritdoc />
    public ICommandSender GetCommandSender()
    {
        return new RoutingCommandSender(this);
    }
}