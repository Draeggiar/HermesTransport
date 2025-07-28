namespace Hermes.Core;

/// <summary>
/// Represents options for message subscription.
/// </summary>
public class SubscriptionOptions
{
    /// <summary>
    /// The subscription group or consumer group name.
    /// Messages are load-balanced across subscribers in the same group.
    /// </summary>
    public string? Group { get; set; }
    
    /// <summary>
    /// Indicates whether the subscription should start from the beginning of the message stream.
    /// </summary>
    public bool FromBeginning { get; set; } = false;
    
    /// <summary>
    /// Maximum number of messages to process concurrently.
    /// </summary>
    public int MaxConcurrency { get; set; } = 1;
    
    /// <summary>
    /// Indicates whether message acknowledgment is required.
    /// </summary>
    public bool AutoAcknowledge { get; set; } = true;
}

/// <summary>
/// Represents a subscription to messages.
/// </summary>
public interface ISubscription : IDisposable
{
    /// <summary>
    /// The unique identifier for this subscription.
    /// </summary>
    string SubscriptionId { get; }
    
    /// <summary>
    /// Indicates whether the subscription is currently active.
    /// </summary>
    bool IsActive { get; }
    
    /// <summary>
    /// Starts the subscription.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Stops the subscription.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines operations for subscribing to messages.
/// </summary>
public interface IMessageSubscriber
{
    /// <summary>
    /// Subscribes to messages of a specific type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
    /// <param name="handler">The handler that will process the messages.</param>
    /// <param name="options">Optional subscription options.</param>
    /// <returns>A subscription that can be used to control the message flow.</returns>
    ISubscription Subscribe<TMessage>(IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage;
    
    /// <summary>
    /// Subscribes to messages on a specific topic.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
    /// <param name="topic">The topic to subscribe to.</param>
    /// <param name="handler">The handler that will process the messages.</param>
    /// <param name="options">Optional subscription options.</param>
    /// <returns>A subscription that can be used to control the message flow.</returns>
    ISubscription Subscribe<TMessage>(string topic, IMessageHandler<TMessage> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage;

    /// <summary>
    /// Subscribes to message envelopes of a specific type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="handler">The handler that will process the message envelopes.</param>
    /// <param name="options">Optional subscription options.</param>
    /// <returns>A subscription that can be used to control the message flow.</returns>
    ISubscription Subscribe<TMessage, TContext>(IMessageEnvelopeHandler<TMessage, TContext> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage;
    
    /// <summary>
    /// Subscribes to message envelopes on a specific topic.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="topic">The topic to subscribe to.</param>
    /// <param name="handler">The handler that will process the message envelopes.</param>
    /// <param name="options">Optional subscription options.</param>
    /// <returns>A subscription that can be used to control the message flow.</returns>
    ISubscription Subscribe<TMessage, TContext>(string topic, IMessageEnvelopeHandler<TMessage, TContext> handler, SubscriptionOptions? options = null) 
        where TMessage : IMessage;
}