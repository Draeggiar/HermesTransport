namespace HermesTransport.Messaging;

/// <summary>
/// Represents a generic message envelope that can carry additional context information
/// along with the actual message payload.
/// </summary>
/// <typeparam name="TMessage">The type of message being wrapped.</typeparam>
/// <typeparam name="TContext">The type of context information to include.</typeparam>
public interface IMessageEnvelope<out TMessage, out TContext> 
    where TMessage : IMessage
{
    /// <summary>
    /// The wrapped message payload.
    /// </summary>
    TMessage Message { get; }
    
    /// <summary>
    /// Additional context information associated with the message.
    /// </summary>
    TContext Context { get; }
    
    /// <summary>
    /// The unique identifier for this envelope.
    /// </summary>
    string EnvelopeId { get; }
    
    /// <summary>
    /// The timestamp when the envelope was created.
    /// </summary>
    DateTimeOffset EnvelopeTimestamp { get; }
}

/// <summary>
/// Represents a message envelope without specific context type constraints.
/// </summary>
/// <typeparam name="TMessage">The type of message being wrapped.</typeparam>
public interface IMessageEnvelope<out TMessage> : IMessageEnvelope<TMessage, object?>
    where TMessage : IMessage
{
}

/// <summary>
/// Base implementation of a message envelope.
/// </summary>
/// <typeparam name="TMessage">The type of message being wrapped.</typeparam>
/// <typeparam name="TContext">The type of context information.</typeparam>
public class MessageEnvelope<TMessage, TContext> : IMessageEnvelope<TMessage, TContext>
    where TMessage : IMessage
{
    /// <summary>
    /// Initializes a new instance of the MessageEnvelope class.
    /// </summary>
    /// <param name="message">The message to wrap.</param>
    /// <param name="context">The context information to include.</param>
    public MessageEnvelope(TMessage message, TContext context)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Context = context;
        EnvelopeId = Guid.NewGuid().ToString();
        EnvelopeTimestamp = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Initializes a new instance of the MessageEnvelope class with a specific envelope ID.
    /// </summary>
    /// <param name="message">The message to wrap.</param>
    /// <param name="context">The context information to include.</param>
    /// <param name="envelopeId">The envelope identifier.</param>
    public MessageEnvelope(TMessage message, TContext context, string envelopeId)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Context = context;
        EnvelopeId = envelopeId ?? throw new ArgumentNullException(nameof(envelopeId));
        EnvelopeTimestamp = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc />
    public TMessage Message { get; }

    /// <inheritdoc />
    public TContext Context { get; }

    /// <inheritdoc />
    public string EnvelopeId { get; }

    /// <inheritdoc />
    public DateTimeOffset EnvelopeTimestamp { get; }
}

/// <summary>
/// Implementation of a message envelope without specific context type constraints.
/// </summary>
/// <typeparam name="TMessage">The type of message being wrapped.</typeparam>
public class MessageEnvelope<TMessage> : MessageEnvelope<TMessage, object?>, IMessageEnvelope<TMessage>
    where TMessage : IMessage
{
    /// <summary>
    /// Initializes a new instance of the MessageEnvelope class.
    /// </summary>
    /// <param name="message">The message to wrap.</param>
    /// <param name="context">The context information to include.</param>
    public MessageEnvelope(TMessage message, object? context = null) 
        : base(message, context)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the MessageEnvelope class with a specific envelope ID.
    /// </summary>
    /// <param name="message">The message to wrap.</param>
    /// <param name="context">The context information to include.</param>
    /// <param name="envelopeId">The envelope identifier.</param>
    public MessageEnvelope(TMessage message, object? context, string envelopeId) 
        : base(message, context, envelopeId)
    {
    }
}