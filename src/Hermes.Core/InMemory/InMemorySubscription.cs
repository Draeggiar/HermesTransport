using System.Collections.Concurrent;
using System.Text.Json;

namespace Hermes.Core.InMemory;

/// <summary>
/// In-memory implementation of a subscription for testing and development purposes.
/// </summary>
internal class InMemorySubscription : ISubscription
{
    private readonly InMemoryMessageBroker _broker;
    private readonly string _topic;
    private readonly Type _messageType;
    private readonly object _handler;
    private readonly SubscriptionOptions _options;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _disposed;

    public InMemorySubscription(
        string subscriptionId,
        InMemoryMessageBroker broker,
        string topic,
        Type messageType,
        object handler,
        SubscriptionOptions options)
    {
        SubscriptionId = subscriptionId;
        _broker = broker;
        _topic = topic;
        _messageType = messageType;
        _handler = handler;
        _options = options;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public string SubscriptionId { get; }
    public bool IsActive { get; private set; }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(InMemorySubscription));

        IsActive = true;
        _broker.AddSubscription(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return Task.CompletedTask;

        IsActive = false;
        _broker.RemoveSubscription(this);
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    internal async Task ProcessMessageAsync(string messageJson, CancellationToken cancellationToken = default)
    {
        if (!IsActive || _disposed)
            return;

        try
        {
            // Deserialize the message
            var message = JsonSerializer.Deserialize(messageJson, _messageType);
            if (message == null)
                return;

            // Get the HandleAsync method from the handler
            var handleMethod = _handler.GetType().GetMethod("HandleAsync");
            if (handleMethod == null)
                return;

            // Invoke the handler
            var result = handleMethod.Invoke(_handler, [message, cancellationToken]);
            if (result is Task task)
            {
                await task;
            }
        }
        catch (Exception)
        {
            // In a real implementation, you might want to handle errors differently
            // For now, we'll just ignore errors to keep the example simple
        }
    }

    internal bool MatchesTopic(string topic) => _topic == topic;
    internal Type GetMessageType() => _messageType;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        StopAsync().Wait(TimeSpan.FromSeconds(5));
        _cancellationTokenSource.Dispose();
    }
}