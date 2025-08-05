namespace HermesTransport;

/// <summary>
/// Extension methods for configuring BrokerRegistry with a fluent API.
/// </summary>
internal static class BrokerRegistryExtensions
{
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