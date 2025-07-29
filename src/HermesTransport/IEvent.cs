namespace HermesTransport;

/// <summary>
/// Represents an event that has occurred in the system.
/// Events are a special type of message that indicate something has happened.
/// </summary>
public interface IEvent : IMessage
{
    /// <summary>
    /// The source or origin of the event.
    /// </summary>
    string Source { get; }
    
    /// <summary>
    /// The version of the event schema.
    /// </summary>
    string Version { get; }
}