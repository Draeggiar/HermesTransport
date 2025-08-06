using HermesTransport.Brokers;
using HermesTransport.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HermesTransport.Configuration;

/// <summary>
/// Extension methods for configuring HermesTransport with dependency injection.
/// </summary>
public static class HermesTransportHostExtensions
{
    /// <summary>
    /// Adds HermesTransport services to the host builder and configures message brokers.
    /// </summary>
    /// <param name="hostBuilder">The host builder to configure.</param>
    /// <param name="configureOptions">Action to configure HermesTransport options.</param>
    /// <returns>The host builder for method chaining.</returns>
    public static IHostBuilder UseHermesTransport(this IHostBuilder hostBuilder, Action<HermesTransportOptions> configureOptions)
    {
        if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        return hostBuilder.ConfigureServices((context, services) =>
        {
            // Configure options
            var options = new HermesTransportOptions();
            configureOptions(options);

            // Register the configured broker registry as singleton
            services.AddSingleton<IBrokerRegistry>(options.Registry);

            // Register the routing publishers and senders
            services.AddTransient<IMessagePublisher>(provider => 
                provider.GetRequiredService<IBrokerRegistry>().GetPublisher());
            
            services.AddTransient<IEventPublisher>(provider => 
                provider.GetRequiredService<IBrokerRegistry>().GetEventPublisher());
            
            services.AddTransient<ICommandSender>(provider => 
                provider.GetRequiredService<IBrokerRegistry>().GetCommandSender());
            
            services.AddTransient<IMessageSubscriber>(provider => 
                provider.GetRequiredService<IBrokerRegistry>().GetSubscriber());
        });
    }
}