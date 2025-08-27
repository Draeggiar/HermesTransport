using HermesTransport.Configuration;
using HermesTransport.Discovery;
using System.Reflection;

namespace HermesTransport.Extensions;

/// <summary>
/// Extension methods for HermesTransportOptions to simplify handler discovery configuration.
/// </summary>
public static class HandlerDiscoveryExtensions
{
    /// <summary>
    /// Enables handler autodiscovery and scans the calling assembly for handlers.
    /// Automatically registers discovered handlers in the DI container.
    /// </summary>
    /// <param name="options">The HermesTransport options.</param>
    /// <returns>The options instance for method chaining.</returns>
    public static HermesTransportOptions EnableAutoDiscovery(this HermesTransportOptions options)
    {
        return options.EnableHandlerDiscoveryWithAutoRegistration(discovery =>
        {
            discovery.ScanCallingAssembly();
        });
    }

    /// <summary>
    /// Enables handler autodiscovery and scans the specified assembly for handlers.
    /// </summary>
    /// <param name="options">The HermesTransport options.</param>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>The options instance for method chaining.</returns>
    public static HermesTransportOptions EnableAutoDiscovery(this HermesTransportOptions options, Assembly assembly)
    {
        return options.EnableHandlerDiscovery(discovery =>
        {
            discovery.ScanAssembly(assembly);
        });
    }

    /// <summary>
    /// Enables handler autodiscovery and scans the specified assemblies for handlers.
    /// </summary>
    /// <param name="options">The HermesTransport options.</param>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The options instance for method chaining.</returns>
    public static HermesTransportOptions EnableAutoDiscovery(this HermesTransportOptions options, params Assembly[] assemblies)
    {
        return options.EnableHandlerDiscovery(discovery =>
        {
            discovery.ScanAssemblies(assemblies);
        });
    }

    /// <summary>
    /// Enables handler autodiscovery and scans assemblies based on the specified type.
    /// </summary>
    /// <typeparam name="T">A type from the assembly to scan.</typeparam>
    /// <param name="options">The HermesTransport options.</param>
    /// <returns>The options instance for method chaining.</returns>
    public static HermesTransportOptions EnableAutoDiscovery<T>(this HermesTransportOptions options)
    {
        return options.EnableHandlerDiscovery(discovery =>
        {
            discovery.ScanAssembly(typeof(T).Assembly);
        });
    }

    /// <summary>
    /// Adds an assembly to scan for handlers. Can be chained with other configuration methods.
    /// </summary>
    /// <param name="options">The HermesTransport options.</param>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>The options instance for method chaining.</returns>
    public static HermesTransportOptions ScanAssembly(this HermesTransportOptions options, Assembly assembly)
    {
        options.HandlerDiscoveryOptions.ScanAssembly(assembly);
        return options;
    }

    /// <summary>
    /// Adds assemblies to scan for handlers. Can be chained with other configuration methods.
    /// </summary>
    /// <param name="options">The HermesTransport options.</param>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The options instance for method chaining.</returns>
    public static HermesTransportOptions ScanAssemblies(this HermesTransportOptions options, params Assembly[] assemblies)
    {
        options.HandlerDiscoveryOptions.ScanAssemblies(assemblies);
        return options;
    }

    /// <summary>
    /// Adds a type filter for handler discovery. Can be chained with other configuration methods.
    /// </summary>
    /// <param name="options">The HermesTransport options.</param>
    /// <param name="filter">The filter to apply.</param>
    /// <returns>The options instance for method chaining.</returns>
    public static HermesTransportOptions WithHandlerFilter(this HermesTransportOptions options, Func<Type, bool> filter)
    {
        options.HandlerDiscoveryOptions.WithTypeFilter(filter);
        return options;
    }
}