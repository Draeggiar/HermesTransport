namespace HermesTransport.Subscriptions;

/// <summary>
///     Represents a subscription to messages.
/// </summary>
public interface ISubscription : IAsyncDisposable
{
    /// <summary>
    ///     The unique identifier for this subscription.
    /// </summary>
    string SubscriptionId { get; }

    /// <summary>
    ///     Indicates whether the subscription is currently active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    ///     Marks the subscription as complete, indicating no further messages will be delivered.
    /// </summary>
    void Complete();

    /// <summary>
    ///     Delivers a message to the subscription asynchronously.
    /// </summary>
    /// <param name="message">The message to deliver.</param>
    /// <param name="cancellation">Token to observe for cancellation requests.</param>
    Task DeliverMessageAsync(IMessage message, CancellationToken cancellation = default);

    /// <summary>
    ///     Starts the subscription.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stops the subscription.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}