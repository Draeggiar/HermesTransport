using HermesTransport.Messaging;
using System.Reflection;

namespace HermesTransport.Discovery;

/// <summary>
/// Defines operations for discovering message handlers in assemblies and the DI container.
/// </summary>
public interface IHandlerDiscoveryService
{
    /// <summary>
    /// Discovers all message handlers in the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <returns>A collection of discovered handler registrations.</returns>
    IEnumerable<HandlerRegistration> DiscoverHandlers(params Assembly[] assemblies);

    /// <summary>
    /// Discovers all message handlers in the specified assemblies with filtering options.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <param name="filter">Optional filter to apply to discovered handlers.</param>
    /// <returns>A collection of discovered handler registrations.</returns>
    IEnumerable<HandlerRegistration> DiscoverHandlers(Assembly[] assemblies, Func<Type, bool>? filter = null);

    /// <summary>
    /// Gets all registered handlers from the DI container for a specific message type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to get handlers for.</typeparam>
    /// <param name="serviceProvider">The service provider to query.</param>
    /// <returns>A collection of handlers for the specified message type.</returns>
    IEnumerable<IMessageHandler<TMessage>> GetRegisteredHandlers<TMessage>(IServiceProvider serviceProvider) 
        where TMessage : IMessage;
}