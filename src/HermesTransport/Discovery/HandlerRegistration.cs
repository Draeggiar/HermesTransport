using HermesTransport.Messaging;

namespace HermesTransport.Discovery;

/// <summary>
/// Represents a discovered message handler registration.
/// </summary>
public class HandlerRegistration
{
    /// <summary>
    /// Gets the type of the handler.
    /// </summary>
    public Type HandlerType { get; }

    /// <summary>
    /// Gets the type of the message that this handler processes.
    /// </summary>
    public Type MessageType { get; }

    /// <summary>
    /// Gets the handler interface type (IMessageHandler, IEventHandler, or ICommandHandler).
    /// </summary>
    public Type HandlerInterfaceType { get; }

    /// <summary>
    /// Gets a value indicating whether this handler processes events.
    /// </summary>
    public bool IsEventHandler => typeof(IEvent).IsAssignableFrom(MessageType);

    /// <summary>
    /// Gets a value indicating whether this handler processes commands.
    /// </summary>
    public bool IsCommandHandler => typeof(ICommand).IsAssignableFrom(MessageType);

    /// <summary>
    /// Gets a value indicating whether this handler processes general messages.
    /// </summary>
    public bool IsMessageHandler => !IsEventHandler && !IsCommandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRegistration"/> class.
    /// </summary>
    /// <param name="handlerType">The type of the handler.</param>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="handlerInterfaceType">The handler interface type.</param>
    public HandlerRegistration(Type handlerType, Type messageType, Type handlerInterfaceType)
    {
        HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
        HandlerInterfaceType = handlerInterfaceType ?? throw new ArgumentNullException(nameof(handlerInterfaceType));
    }

    /// <summary>
    /// Creates a handler instance from the DI container.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve the handler from.</param>
    /// <returns>The handler instance.</returns>
    public object CreateHandler(IServiceProvider serviceProvider)
    {
        var handler = serviceProvider.GetService(HandlerType);
        if (handler != null)
        {
            return handler;
        }

        // Fallback to direct instantiation if not registered in DI
        try
        {
            return Activator.CreateInstance(HandlerType) 
                   ?? throw new InvalidOperationException($"Could not create instance of handler type {HandlerType.Name}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Could not create instance of handler type {HandlerType.Name}. " +
                $"Make sure it's registered in the DI container or has a parameterless constructor.", ex);
        }
    }

    /// <summary>
    /// Creates a strongly-typed handler instance from the DI container.
    /// </summary>
    /// <typeparam name="TMessage">The type of message the handler processes.</typeparam>
    /// <param name="serviceProvider">The service provider to resolve the handler from.</param>
    /// <returns>The strongly-typed handler instance.</returns>
    public IMessageHandler<TMessage> CreateHandler<TMessage>(IServiceProvider serviceProvider) 
        where TMessage : IMessage
    {
        var handler = CreateHandler(serviceProvider);
        return (IMessageHandler<TMessage>)handler;
    }
}