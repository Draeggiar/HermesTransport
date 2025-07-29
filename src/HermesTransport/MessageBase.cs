namespace HermesTransport;

/// <summary>
/// Base implementation of a message with common properties.
/// </summary>
public abstract class MessageBase : IMessage
{
    protected MessageBase()
    {
        MessageId = Guid.NewGuid().ToString();
        Timestamp = DateTimeOffset.UtcNow;
        MessageType = GetType().Name;
    }
    
    protected MessageBase(string? correlationId) : this()
    {
        CorrelationId = correlationId;
    }

    /// <inheritdoc />
    public string MessageId { get; protected set; }

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; protected set; }

    /// <inheritdoc />
    public virtual string MessageType { get; protected set; }

    /// <inheritdoc />
    public string? CorrelationId { get; protected set; }
}

/// <summary>
/// Base implementation of an event with common properties.
/// </summary>
public abstract class EventBase : MessageBase, IEvent
{
    protected EventBase(string source, string version = "1.0") : base()
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }
    
    protected EventBase(string source, string version, string? correlationId) : base(correlationId)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }

    /// <inheritdoc />
    public string Source { get; protected set; }

    /// <inheritdoc />
    public string Version { get; protected set; }
}

/// <summary>
/// Base implementation of a command with common properties.
/// </summary>
public abstract class CommandBase : MessageBase, ICommand
{
    protected CommandBase(string target, string action) : base()
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }
    
    protected CommandBase(string target, string action, string? correlationId) : base(correlationId)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /// <inheritdoc />
    public string Target { get; protected set; }

    /// <inheritdoc />
    public string Action { get; protected set; }
}