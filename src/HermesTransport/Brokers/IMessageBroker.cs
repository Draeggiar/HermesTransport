using HermesTransport.Messaging;

namespace HermesTransport.Brokers;

/// <summary>
/// Defines the contract for message brokers that handle the transport and storage of messages.
/// External systems like RabbitMQ, SQL Server, or in-memory implementations will implement this interface.
/// </summary>
public interface IMessageBroker : IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the broker is connected and ready to process messages.
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// Establishes a connection to the message broker.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ConnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Disconnects from the message broker.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a topic or ensures it exists in the broker.
    /// </summary>
    /// <param name="topic">The name of the topic to create.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateTopicAsync(string topic, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a topic from the broker if it exists.
    /// </summary>
    /// <param name="topic">The name of the topic to delete.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteTopicAsync(string topic, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a list of all available topics in the broker.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task containing the list of topic names.</returns>
    Task<IEnumerable<string>> GetTopicsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a message publisher instance from this broker.
    /// </summary>
    /// <returns>A message publisher that uses this broker for transport.</returns>
    IMessagePublisher GetPublisher();

    /// <summary>
    /// Gets a message subscriber instance from this broker.
    /// </summary>
    /// <returns>A message subscriber that uses this broker for transport.</returns>
    IMessageSubscriber GetSubscriber();

    /// <summary>
    /// Gets an event publisher instance from this broker.
    /// </summary>
    /// <returns>An event publisher that uses this broker for transport.</returns>
    IEventPublisher GetEventPublisher();

    /// <summary>
    /// Gets a command sender instance from this broker.
    /// </summary>
    /// <returns>A command sender that uses this broker for transport.</returns>
    ICommandSender GetCommandSender();
}