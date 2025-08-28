using HermesTransport.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HermesTransport.Discovery;

/// <summary>
/// Default implementation of the handler discovery service.
/// </summary>
public class HandlerDiscoveryService : IHandlerDiscoveryService
{
    /// <inheritdoc />
    public IEnumerable<HandlerRegistration> DiscoverHandlers(params Assembly[] assemblies)
    {
        return DiscoverHandlers(assemblies, null);
    }

    /// <inheritdoc />
    public IEnumerable<HandlerRegistration> DiscoverHandlers(Assembly[] assemblies, Func<Type, bool>? filter = null)
    {
        if (assemblies == null || assemblies.Length == 0)
            return Enumerable.Empty<HandlerRegistration>();

        var registrations = new List<HandlerRegistration>();

        foreach (var assembly in assemblies)
        {
            var types = GetTypesFromAssembly(assembly);
            
            foreach (var type in types)
            {
                if (filter != null && !filter(type))
                    continue;

                var handlerRegistrations = GetHandlerRegistrations(type);
                registrations.AddRange(handlerRegistrations);
            }
        }

        return registrations;
    }

    /// <inheritdoc />
    public IEnumerable<IMessageHandler<TMessage>> GetRegisteredHandlers<TMessage>(IServiceProvider serviceProvider) 
        where TMessage : IMessage
    {
        try
        {
            return serviceProvider.GetServices<IMessageHandler<TMessage>>();
        }
        catch
        {
            return Enumerable.Empty<IMessageHandler<TMessage>>();
        }
    }

    private static Type[] GetTypesFromAssembly(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Return only the types that loaded successfully
            return ex.Types.Where(t => t != null).Cast<Type>().ToArray();
        }
    }

    private static IEnumerable<HandlerRegistration> GetHandlerRegistrations(Type type)
    {
        if (!type.IsClass || type.IsAbstract || type.IsInterface)
            yield break;

        var interfaces = type.GetInterfaces();

        foreach (var interfaceType in interfaces)
        {
            if (!interfaceType.IsGenericType)
                continue;

            var genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
            
            // Check if it's a message handler interface
            if (genericTypeDefinition == typeof(IMessageHandler<>) ||
                genericTypeDefinition == typeof(IEventHandler<>) ||
                genericTypeDefinition == typeof(ICommandHandler<>))
            {
                var messageType = interfaceType.GetGenericArguments()[0];
                
                // Ensure the message type implements IMessage
                if (typeof(IMessage).IsAssignableFrom(messageType))
                {
                    yield return new HandlerRegistration(type, messageType, interfaceType);
                }
            }
        }
    }
}