using System.Collections.Concurrent;
using System.Text.Json;

namespace Hermes.Core.InMemory;

/// <summary>
/// In-memory implementation of a message broker for testing and development purposes.
/// This implementation stores messages in memory and processes them synchronously.
/// </summary>
public class InMemoryMessageBroker : IMessageBroker
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _topics;
    private readonly ConcurrentDictionary<string, InMemorySubscription> _subscriptions;
    private readonly object _lock = new();
    private bool _disposed;

    public InMemoryMessageBroker()
    {
        _topics = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        _subscriptions = new ConcurrentDictionary<string, InMemorySubscription>();
        IsConnected = true; // In-memory broker is always "connected"
    }

    public bool IsConnected { get; private set; }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        IsConnected = true;
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public Task CreateTopicAsync(string topic, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        _topics.TryAdd(topic, new ConcurrentQueue<string>());
        return Task.CompletedTask;
    }

    public Task DeleteTopicAsync(string topic, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        _topics.TryRemove(topic, out _);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetTopicsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<string>>(_topics.Keys.ToList());
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        
        var topic = message.MessageType;
        await PublishAsync(topic, message, cancellationToken);
    }

    public async Task PublishAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(message);

        if (_disposed)
            throw new ObjectDisposedException(nameof(InMemoryMessageBroker));

        // Ensure topic exists
        await CreateTopicAsync(topic, cancellationToken);

        // Serialize the message
        var messageJson = JsonSerializer.Serialize(message, message.GetType());

        // Add to the topic queue
        if (_topics.TryGetValue(topic, out var queue))
        {
            queue.Enqueue(messageJson);
        }

        // Process the message for all matching subscriptions
        await ProcessMessageForSubscriptionsAsync(topic, messageJson, cancellationToken);
    }

    public ISubscription Subscribe<TMessage>(IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage
    {
        ArgumentNullException.ThrowIfNull(handler);
        
        var messageType = typeof(TMessage);
        var topic = messageType.Name;
        
        return Subscribe(topic, handler, options);
    }

    public ISubscription Subscribe<TMessage>(string topic, IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(handler);

        if (_disposed)
            throw new ObjectDisposedException(nameof(InMemoryMessageBroker));

        options ??= new SubscriptionOptions();
        var subscriptionId = Guid.NewGuid().ToString();
        var messageType = typeof(TMessage);

        var subscription = new InMemorySubscription(
            subscriptionId,
            this,
            topic,
            messageType,
            handler,
            options);

        _subscriptions.TryAdd(subscriptionId, subscription);

        return subscription;
    }

    internal void AddSubscription(InMemorySubscription subscription)
    {
        // Subscription is already added in the Subscribe method
        // This method is called when the subscription starts
    }

    internal void RemoveSubscription(InMemorySubscription subscription)
    {
        _subscriptions.TryRemove(subscription.SubscriptionId, out _);
    }

    private async Task ProcessMessageForSubscriptionsAsync(string topic, string messageJson, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var subscription in _subscriptions.Values)
        {
            if (subscription.MatchesTopic(topic) && subscription.IsActive)
            {
                tasks.Add(subscription.ProcessMessageAsync(messageJson, cancellationToken));
            }
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        // Stop all subscriptions
        var disposeTasks = _subscriptions.Values.Select(s => Task.Run(() => s.Dispose())).ToArray();
        Task.WaitAll(disposeTasks, TimeSpan.FromSeconds(10));

        _subscriptions.Clear();
        _topics.Clear();
        IsConnected = false;
    }
}