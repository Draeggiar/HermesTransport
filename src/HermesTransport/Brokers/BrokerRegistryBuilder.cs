namespace HermesTransport.Brokers;

internal class BrokerRegistryBuilder
{
    private Func<IServiceProvider, IMessageBroker>? _commandsBrokerFactory;
    private Func<IServiceProvider, IMessageBroker>? _eventsBrokerFactory;
    private Func<IServiceProvider, IMessageBroker>? _messagesBrokerFactory;

    public BrokerRegistryBuilder RegisterCommandBroker(Func<IServiceProvider, IMessageBroker> commandBrokerFactory)
    {
        _commandsBrokerFactory = commandBrokerFactory ?? throw new ArgumentNullException(nameof(commandBrokerFactory));
        return this;
    }

    public BrokerRegistryBuilder RegisterEventBroker(Func<IServiceProvider, IMessageBroker> eventBrokerFactory)
    {
        _eventsBrokerFactory = eventBrokerFactory ?? throw new ArgumentNullException(nameof(eventBrokerFactory));
        return this;
    }

    public BrokerRegistryBuilder RegisterMessageBroker(Func<IServiceProvider, IMessageBroker> messageBrokerFactory)
    {
        _messagesBrokerFactory = messageBrokerFactory ?? throw new ArgumentNullException(nameof(messageBrokerFactory));
        return this;
    }
    
    public IBrokerRegistry Build(IServiceProvider services)
    {
        if (_commandsBrokerFactory is null)
            throw new InvalidOperationException("Command broker factory must be registered.");

        if (_eventsBrokerFactory is null)
            throw new InvalidOperationException("Event broker factory must be registered.");

        if (_messagesBrokerFactory is null)
            throw new InvalidOperationException("Message broker factory must be registered.");

        var commandBroker = _commandsBrokerFactory(services);
        var eventBroker = _eventsBrokerFactory(services);
        var messageBroker = _messagesBrokerFactory(services);

        return new BrokerRegistry(eventBroker, commandBroker, messageBroker);
    }
}