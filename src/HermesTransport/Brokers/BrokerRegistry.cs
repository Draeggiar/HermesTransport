using HermesTransport.Messaging;
using HermesTransport.Subscriptions;

namespace HermesTransport.Brokers;

/// <summary>
///     Default implementation of IBrokerRegistry that manages multiple message brokers.
/// </summary>
internal class BrokerRegistry : IBrokerRegistry
{
    private readonly IMessageBroker _commandBroker;
    private readonly IMessageBroker _eventBroker;
    private readonly IMessageBroker _messageBroker;

    public BrokerRegistry(IMessageBroker eventBroker, IMessageBroker commandBroker, IMessageBroker messageBroker)
    {
        _eventBroker = eventBroker ?? throw new ArgumentNullException(nameof(eventBroker));
        _commandBroker = commandBroker ?? throw new ArgumentNullException(nameof(commandBroker));
        _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
    }

    /// <inheritdoc />
    public IMessageBroker? GetEventBroker() => _eventBroker;

    /// <inheritdoc />
    public IMessageBroker? GetCommandBroker() => _commandBroker;

    /// <inheritdoc />
    public IMessageBroker? GetMessageBroker() => _messageBroker;

    /// <inheritdoc />
    public IMessagePublisher GetPublisher() => new RoutingMessagePublisher(this);

    /// <inheritdoc />
    public IMessageSubscriber GetSubscriber() => new RoutingMessageSubscriber(this);

    /// <inheritdoc />
    public IEventPublisher GetEventPublisher() => new RoutingEventPublisher(this);

    /// <inheritdoc />
    public ICommandSender GetCommandSender() => new RoutingCommandSender(this);
}