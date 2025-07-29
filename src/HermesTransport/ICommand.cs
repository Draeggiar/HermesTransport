namespace HermesTransport;

/// <summary>
/// Represents a command that requests an action to be performed.
/// Commands are a special type of message that request state changes.
/// </summary>
public interface ICommand : IMessage
{
    /// <summary>
    /// The intended recipient or target of the command.
    /// </summary>
    string Target { get; }
    
    /// <summary>
    /// The requested action or operation.
    /// </summary>
    string Action { get; }
}