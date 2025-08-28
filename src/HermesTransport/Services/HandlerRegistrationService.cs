using HermesTransport.Brokers;
using HermesTransport.Discovery;
using HermesTransport.Messaging;
using HermesTransport.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace HermesTransport.Services;

/// <summary>
/// Background service that automatically registers discovered handlers with the message brokers.
/// </summary>
public class HandlerRegistrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHandlerDiscoveryService _discoveryService;
    private readonly ILogger<HandlerRegistrationService> _logger;
    private readonly HandlerDiscoveryOptions _options;
    private readonly List<ISubscription> _subscriptions = new();

    public HandlerRegistrationService(
        IServiceProvider serviceProvider,
        IHandlerDiscoveryService discoveryService,
        ILogger<HandlerRegistrationService> logger,
        HandlerDiscoveryOptions options)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _discoveryService = discoveryService ?? throw new ArgumentNullException(nameof(discoveryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.IsEnabled)
        {
            _logger.LogInformation("Handler autodiscovery is disabled");
            return;
        }

        _logger.LogInformation("Starting handler autodiscovery");

        try
        {
            await RegisterDiscoveredHandlers(cancellationToken);
            _logger.LogInformation("Handler autodiscovery completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during handler autodiscovery");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping handler registrations");

        foreach (var subscription in _subscriptions)
        {
            try
            {
                await subscription.StopAsync(cancellationToken);
                await subscription.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error stopping handler subscription");
            }
        }

        _subscriptions.Clear();
        _logger.LogInformation("Handler registrations stopped");
    }

    private async Task RegisterDiscoveredHandlers(CancellationToken cancellationToken)
    {
        var assemblies = _options.AssembliesToScan.ToArray();
        if (assemblies.Length == 0)
        {
            _logger.LogWarning("No assemblies configured for handler discovery");
            return;
        }

        var handlerRegistrations = _discoveryService.DiscoverHandlers(assemblies, _options.TypeFilter);
        var brokerRegistry = _serviceProvider.GetRequiredService<IBrokerRegistry>();

        var registeredCount = 0;
        foreach (var registration in handlerRegistrations)
        {
            try
            {
                // Check if the handler type is actually registered in DI
                var handler = _serviceProvider.GetService(registration.HandlerType);
                if (handler != null)
                {
                    await RegisterHandler(registration, brokerRegistry, cancellationToken);
                    registeredCount++;
                }
                else
                {
                    _logger.LogDebug("Handler type {HandlerType} not registered in DI container, skipping", 
                        registration.HandlerType.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register handler {HandlerType} for message {MessageType}", 
                    registration.HandlerType.Name, registration.MessageType.Name);
                
                if (!_options.ContinueOnError)
                    throw;
            }
        }

        _logger.LogInformation("Successfully registered {Count} handlers from {AssemblyCount} assemblies", 
            registeredCount, assemblies.Length);
    }

    private async Task RegisterHandler(HandlerRegistration registration, IBrokerRegistry brokerRegistry, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Registering handler {HandlerType} for message {MessageType}", 
            registration.HandlerType.Name, registration.MessageType.Name);

        // Use reflection to call the generic registration method
        var method = GetType().GetMethod(nameof(RegisterHandlerGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
        var genericMethod = method!.MakeGenericMethod(registration.MessageType);
        
        var task = (Task)genericMethod.Invoke(this, new object[] { registration, brokerRegistry, cancellationToken })!;
        await task;
    }

    private Task RegisterHandlerGeneric<TMessage>(HandlerRegistration registration, IBrokerRegistry brokerRegistry, CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        var subscriber = brokerRegistry.GetSubscriber();
        var handler = registration.CreateHandler<TMessage>(_serviceProvider);

        var subscription = subscriber.Subscribe(handler, _options.DefaultSubscriptionOptions);
        _subscriptions.Add(subscription);

        _logger.LogInformation("Registered handler {HandlerType} for message {MessageType}", 
            registration.HandlerType.Name, registration.MessageType.Name);

        return Task.CompletedTask;
    }
}