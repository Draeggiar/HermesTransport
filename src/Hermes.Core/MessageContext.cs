namespace Hermes.Core;

/// <summary>
/// Represents common context information that can be included with messages.
/// </summary>
public class MessageContext
{
    /// <summary>
    /// Unique identifier to correlate related messages or operations.
    /// </summary>
    public string? CorrelationId { get; set; }
    
    /// <summary>
    /// Information about the user who initiated the message.
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// The name or identifier of the user who initiated the message.
    /// </summary>
    public string? UserName { get; set; }
    
    /// <summary>
    /// The session identifier associated with the message.
    /// </summary>
    public string? SessionId { get; set; }
    
    /// <summary>
    /// The request identifier from which this message originated.
    /// </summary>
    public string? RequestId { get; set; }
    
    /// <summary>
    /// The IP address of the client that initiated the message.
    /// </summary>
    public string? ClientIpAddress { get; set; }
    
    /// <summary>
    /// The user agent string of the client that initiated the message.
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Additional custom properties that can be included with the message.
    /// </summary>
    public Dictionary<string, object?> Properties { get; set; } = new();
}

/// <summary>
/// Provides helper methods for creating message envelopes and working with message context.
/// </summary>
public static class MessageEnvelopeExtensions
{
    /// <summary>
    /// Wraps a message in an envelope with the specified context.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to wrap.</typeparam>
    /// <typeparam name="TContext">The type of context information.</typeparam>
    /// <param name="message">The message to wrap.</param>
    /// <param name="context">The context information to include.</param>
    /// <returns>A message envelope containing the message and context.</returns>
    public static MessageEnvelope<TMessage, TContext> WithContext<TMessage, TContext>(this TMessage message, TContext context)
        where TMessage : IMessage
    {
        return new MessageEnvelope<TMessage, TContext>(message, context);
    }
    
    /// <summary>
    /// Wraps a message in an envelope with message context.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to wrap.</typeparam>
    /// <param name="message">The message to wrap.</param>
    /// <param name="context">The message context to include.</param>
    /// <returns>A message envelope containing the message and context.</returns>
    public static MessageEnvelope<TMessage, MessageContext> WithContext<TMessage>(this TMessage message, MessageContext context)
        where TMessage : IMessage
    {
        return new MessageEnvelope<TMessage, MessageContext>(message, context);
    }
    
    /// <summary>
    /// Wraps a message in an envelope with a correlation ID.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to wrap.</typeparam>
    /// <param name="message">The message to wrap.</param>
    /// <param name="correlationId">The correlation ID to include.</param>
    /// <returns>A message envelope containing the message and correlation context.</returns>
    public static MessageEnvelope<TMessage, MessageContext> WithCorrelationId<TMessage>(this TMessage message, string correlationId)
        where TMessage : IMessage
    {
        return new MessageEnvelope<TMessage, MessageContext>(message, new MessageContext { CorrelationId = correlationId });
    }
    
    /// <summary>
    /// Wraps a message in an envelope with user information.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to wrap.</typeparam>
    /// <param name="message">The message to wrap.</param>
    /// <param name="userId">The user ID to include.</param>
    /// <param name="userName">The user name to include.</param>
    /// <returns>A message envelope containing the message and user context.</returns>
    public static MessageEnvelope<TMessage, MessageContext> WithUser<TMessage>(this TMessage message, string userId, string? userName = null)
        where TMessage : IMessage
    {
        return new MessageEnvelope<TMessage, MessageContext>(message, new MessageContext { UserId = userId, UserName = userName });
    }
    
    /// <summary>
    /// Wraps a message in an envelope with request information.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to wrap.</typeparam>
    /// <param name="message">The message to wrap.</param>
    /// <param name="requestId">The request ID to include.</param>
    /// <param name="correlationId">The correlation ID to include.</param>
    /// <param name="userId">The user ID to include.</param>
    /// <returns>A message envelope containing the message and request context.</returns>
    public static MessageEnvelope<TMessage, MessageContext> WithRequestContext<TMessage>(
        this TMessage message, 
        string? requestId = null, 
        string? correlationId = null, 
        string? userId = null)
        where TMessage : IMessage
    {
        return new MessageEnvelope<TMessage, MessageContext>(message, new MessageContext 
        { 
            RequestId = requestId,
            CorrelationId = correlationId,
            UserId = userId
        });
    }
}