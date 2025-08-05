namespace HermesTransport;

/// <summary>
/// Extension methods for configuring BrokerRegistry with a fluent API.
/// </summary>
public static class BrokerRegistryExtensions
{
    /// <summary>
    /// Creates a new BrokerRegistry instance.
    /// </summary>
    /// <returns>A new BrokerRegistry instance.</returns>
    public static IBrokerRegistry Create()
    {
        return new BrokerRegistry();
    }
    
    /// <summary>
    /// Configures multiple brokers using a fluent configuration action.
    /// </summary>
    /// <param name="registry">The broker registry to configure.</param>
    /// <param name="configure">Configuration action to set up brokers.</param>
    /// <returns>The configured registry instance.</returns>
    public static IBrokerRegistry Configure(this IBrokerRegistry registry, Action<IBrokerRegistry> configure)
    {
        configure?.Invoke(registry);
        return registry;
    }
    
    /// <summary>
    /// Registers the same broker for all message types (events, commands, and messages).
    /// </summary>
    /// <param name="registry">The broker registry.</param>
    /// <param name="broker">The broker to register for all message types.</param>
    /// <returns>The registry instance for method chaining.</returns>
    public static IBrokerRegistry RegisterBrokerForAll(this IBrokerRegistry registry, IMessageBroker broker)
    {
        return registry
            .RegisterEventBroker(broker)
            .RegisterCommandBroker(broker)
            .RegisterMessageBroker(broker);
    }
}