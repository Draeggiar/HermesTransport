using HermesTransport.Subscriptions;
using System.Reflection;

namespace HermesTransport.Discovery;

/// <summary>
/// Configuration options for handler autodiscovery.
/// </summary>
public class HandlerDiscoveryOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether handler autodiscovery is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to continue registration when an error occurs.
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Gets the list of assemblies to scan for handlers.
    /// </summary>
    public List<Assembly> AssembliesToScan { get; } = new();

    /// <summary>
    /// Gets or sets the default subscription options to use for discovered handlers.
    /// </summary>
    public SubscriptionOptions? DefaultSubscriptionOptions { get; set; }

    /// <summary>
    /// Gets or sets an optional filter to apply when discovering handler types.
    /// </summary>
    public Func<Type, bool>? TypeFilter { get; set; }

    /// <summary>
    /// Adds an assembly to scan for handlers.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HandlerDiscoveryOptions ScanAssembly(Assembly assembly)
    {
        if (assembly != null && !AssembliesToScan.Contains(assembly))
        {
            AssembliesToScan.Add(assembly);
        }
        return this;
    }

    /// <summary>
    /// Adds multiple assemblies to scan for handlers.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HandlerDiscoveryOptions ScanAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            ScanAssembly(assembly);
        }
        return this;
    }

    /// <summary>
    /// Adds the calling assembly to scan for handlers.
    /// </summary>
    /// <returns>The options instance for method chaining.</returns>
    public HandlerDiscoveryOptions ScanCallingAssembly()
    {
        return ScanAssembly(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Adds the entry assembly to scan for handlers.
    /// </summary>
    /// <returns>The options instance for method chaining.</returns>
    public HandlerDiscoveryOptions ScanEntryAssembly()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            ScanAssembly(entryAssembly);
        }
        return this;
    }

    /// <summary>
    /// Sets a filter to apply when discovering handler types.
    /// </summary>
    /// <param name="filter">The filter function.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HandlerDiscoveryOptions WithTypeFilter(Func<Type, bool> filter)
    {
        TypeFilter = filter;
        return this;
    }

    /// <summary>
    /// Sets the default subscription options for discovered handlers.
    /// </summary>
    /// <param name="options">The subscription options.</param>
    /// <returns>The options instance for method chaining.</returns>
    public HandlerDiscoveryOptions WithDefaultSubscriptionOptions(SubscriptionOptions options)
    {
        DefaultSubscriptionOptions = options;
        return this;
    }
}