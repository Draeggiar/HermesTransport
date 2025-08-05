namespace HermesTransport;

/// <summary>
/// A message subscriber that routes subscription requests to the appropriate broker based on message type.
/// </summary>
internal class RoutingMessageSubscriber : IMessageSubscriber
{
    private readonly IBrokerRegistry _brokerRegistry;

    public RoutingMessageSubscriber(IBrokerRegistry brokerRegistry)
    {
        _brokerRegistry = brokerRegistry ?? throw new ArgumentNullException(nameof(brokerRegistry));
    }

    /// <inheritdoc />
    public ISubscription Subscribe<TMessage>(IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage
    {
        var broker = GetBrokerForMessage<TMessage>();
        if (broker == null)
        {
            throw new InvalidOperationException($"No broker registered for message type: {typeof(TMessage).Name}");
        }

        var subscriber = broker.GetSubscriber();
        return subscriber.Subscribe(handler, options);
    }

    /// <inheritdoc />
    public ISubscription Subscribe<TMessage>(string topic, IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage
    {
        var broker = GetBrokerForMessage<TMessage>();
        if (broker == null)
        {
            throw new InvalidOperationException($"No broker registered for message type: {typeof(TMessage).Name}");
        }

        var subscriber = broker.GetSubscriber();
        return subscriber.Subscribe(topic, handler, options);
    }

    private IMessageBroker? GetBrokerForMessage<TMessage>() where TMessage : IMessage
    {
        // Route events to event broker
        if (typeof(IEvent).IsAssignableFrom(typeof(TMessage)))
        {
            return _brokerRegistry.GetEventBroker();
        }
        
        // Route commands to command broker
        if (typeof(ICommand).IsAssignableFrom(typeof(TMessage)))
        {
            return _brokerRegistry.GetCommandBroker();
        }
        
        // Route general messages to message broker
        return _brokerRegistry.GetMessageBroker();
    }
}