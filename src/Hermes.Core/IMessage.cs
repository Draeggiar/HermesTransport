namespace Hermes.Core;

/// <summary>
/// Represents a message in the event-driven architecture.
/// This is the base interface for all messages that can be sent through the system.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Unique identifier for the message.
    /// </summary>
    string MessageId { get; }
    
    /// <summary>
    /// The timestamp when the message was created.
    /// </summary>
    DateTimeOffset Timestamp { get; }
    
    /// <summary>
    /// The type of the message. Used for routing and deserialization.
    /// </summary>
    string MessageType { get; }
    
    /// <summary>
    /// Optional correlation identifier for tracking related messages.
    /// </summary>
    string? CorrelationId { get; }
}