using HermesTransport.Brokers;
using HermesTransport.Messaging;

namespace HermesTransport.Subscriptions;

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
        var broker = _brokerRegistry.GetBrokerForMessage<TMessage>();
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
        var broker = _brokerRegistry.GetBrokerForMessage<TMessage>();
        if (broker == null)
        {
            throw new InvalidOperationException($"No broker registered for message type: {typeof(TMessage).Name}");
        }

        var subscriber = broker.GetSubscriber();
        return subscriber.Subscribe(topic, handler, options);
    }
}